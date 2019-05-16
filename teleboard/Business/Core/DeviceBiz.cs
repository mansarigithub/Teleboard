using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Teleboard.Common.ExtensionMethod;
using Teleboard.DataAccess.Context;
using Teleboard.PresentationModel.Model.Device;
using Teleboard.Common.Data;
using System.Data.Entity;
using Teleboard.Mapper.Core;
using Teleboard.PresentationModel.Model.Content;
using Teleboard.DomainModel.Core;
using Teleboard.Common.Exception;
using Teleboard.Localization;
using Teleboard.Models.PresentationModel.Model.TimeBox;
using Teleboard.Common.Enum;
using Z.EntityFramework.Plus;

namespace Teleboard.Business.Core
{
    public class DeviceBiz : BizBase<Device>
    {
        private ApplicationDbContext Context { get; set; }

        public DeviceBiz(ApplicationDbContext context) : base(context)
        {
            Context = context;
        }

        public DataSourceResult ReadDevices(IPrincipal user, DataSourceRequest request)
        {
            var userId = user.Identity.GetUserId();
            var deviceQuery = Context.Devices
                .OrderByDescending(d => d.LastConnectedUtc)
                .Include(device => device.Tenant);
            if (!user.IsInRole(AppRole.Host.ToString()))
                deviceQuery = deviceQuery.Where(device => device.Tenant.TenantUsers.Any(tu => tu.UserId == userId));

            return deviceQuery
                .ApplyRequest(request)
                .MapTo<DeviceDetailsForDeviceListPagePM>()
                .ToDataSourceResult(request);
        }

        public DataSourceResult ReadDevicesWithDetails(IPrincipal user, DataSourceRequest request)
        {
            var userId = user.Identity.GetUserId();
            var deviceQuery = Context.Devices
                .OrderByDescending(d => d.LastConnectedUtc)
                .Include(device => device.Tenant);
            if (!user.IsInRole(AppRole.Host.ToString()))
                deviceQuery = deviceQuery.Where(device => device.Tenant.TenantUsers.Any(tu => tu.UserId == userId));

            var devicesCount = deviceQuery.Count();
            deviceQuery = deviceQuery.ApplyRequest(request);
            var devices = deviceQuery
                .MapTo<DeviceDetailsForDeviceListPagePM>()
                .ToList();
            var deviceIds = devices.Select(d => d.Id).ToArray();
            var localWeekDays = devices.Select(d => DateTime.UtcNow.FromUtcToLocal(d.TimeZoneId).DayOfWeek.ToString()).Distinct().ToArray();

            var timeBoxes = Context.TimeBoxes
                .Include(TenantBiz => TenantBiz.Channel)
                .Where(timeBox => deviceIds.Contains(timeBox.DeviceId) && localWeekDays.Contains(timeBox.WeekDay))
                .Select(timebox => new
                {
                    deviceId = timebox.DeviceId,
                    channelId = timebox.ChannelId,
                    channelName = timebox.Channel.Name,
                    weekDay = timebox.WeekDay,
                    fromHour = timebox.FromHour,
                    fromMinute = timebox.FromMinute,
                    toHour = timebox.ToHour,
                    toMinute = timebox.ToMinute,
                }).ToList();
            devices.ForEach(device =>
            {
                device.LastConnectedLocal = device.LastConnectedUtc?.FromUtcToLocal(device.TimeZoneId);
                var deviceLocalTime = DateTime.UtcNow.FromUtcToLocal(device.TimeZoneId);
                var deviceTimeOdDay = deviceLocalTime.TimeOfDay;
                var timeBox = timeBoxes.SingleOrDefault(tb =>
                    tb.deviceId == device.Id &&
                    tb.weekDay == deviceLocalTime.DayOfWeek.ToString() &&
                    new TimeSpan(tb.fromHour, tb.fromMinute, 0) < deviceTimeOdDay && deviceTimeOdDay < new TimeSpan(tb.toHour, tb.toMinute, 0));
                if (timeBox != null)
                {
                    device.CurrentPlayingChannelId = timeBox.channelId;
                    device.CurrentPlayingChannelName = timeBox.channelName;
                }
            });

            return devices.ToDataSourceResult(devicesCount);
        }

        public void RemoveAllSchedulings(int deviceId, ApplicationUser user)
        {
            if (user.IsHostAdmin || UserOwnsDevice(user.Id, deviceId))
                RemoveAllSchedulings(deviceId);
            else
                throw new BusinessException(BusinessExceptionType.OperationNotAllowed);
        }

        public void RemoveAllSchedulings(int deviceId)
        {
            Context.TimeBoxes
                .Where(tb => tb.Device.Id == deviceId)
                .ToList()
                .ForEach(tb => Context.TimeBoxes.Remove(tb));
            Context.SaveChanges();
        }

        public bool UserOwnsDevice(string userId, int deviceId)
        {
            return Context.Devices.Any(d =>
                d.Id == deviceId &&
                d.Tenant.TenantUsers.Any(tu => tu.UserId == userId));
        }


        public IEnumerable<DevicePM> ReadTenantDevices(int tenantId)
        {
            return Context.Devices
                .Where(device => device.TenantId == tenantId)
                .MapTo<DevicePM>()
                .ToList();
        }

        public void ScheduleDevice(int deviceId, List<EventPM> events, out string saveTime, bool saveChanges = true)
        {
            var departedEvents = new List<EventPM>();
            var device = Context.Devices.Include(d => d.TimeBoxes).Single(d => d.Id == deviceId);
            Context.TimeBoxes.RemoveRange(device.TimeBoxes);

            if (events.Any(x => x.Start > x.End))
                throw new BusinessException(Resources.InvalidDateRange);

            events.ForEach(e => departedEvents.AddRange(DepartEvent(e)));
            departedEvents.ForEach(e =>
            {
                Context.TimeBoxes.Add(new TimeBox
                {
                    ChannelId = e.ChannelId,
                    DeviceId = deviceId,
                    FromHour = e.Start.Hour,
                    FromMinute = e.Start.Minute,
                    ToHour = (e.End.Hour == 0 && e.End.Date > e.Start.Date ? 24 : e.End.Hour),
                    ToMinute = e.End.Minute,
                    WeekDay = e.Start.DayOfWeek.ToString(),
                    PlayAdvertisement = e.PlayAdvertisement,
                    Flag = true,
                });
            });

            saveTime = saveChanges ? DateTime.UtcNow.FromUtcToLocal(device.TimeZoneId).ToLongTimeString() : "";
            if (saveChanges)
            {
                Context.SaveChanges();
            }
        }

        public void ScheduleDevices(IEnumerable<int> deviceIds, List<EventPM> events)
        {
            var saveTime = "";
            deviceIds.ForEach(d => ScheduleDevice(d, events, out saveTime, saveChanges: false));
            Context.SaveChanges();
        }

        public TimeBoxForAdvertisementPM UpdateChannelAdvertisements(ApplicationUser user, int timeboxId, IEnumerable<ContentPM> selectedContents, out string saveTime)
        {
            var userId = user.Id;
            var timebox = Context.TimeBoxes.Include(tb => tb.Device).Single(tb => tb.Id == timeboxId);
            var device = timebox.Device;
            var adsTotallDuration = ComputeChannelAdvertisementsDuration(user, timebox, selectedContents);
            if (timebox.Duration.TotalSeconds < adsTotallDuration)
                throw new BusinessException(Resources.ContentsDurationExceededTimeBoxDuration);

            var lastSequence = Context.ChannelContents.Where(cc => cc.ChannelId == timebox.ChannelId).Select(cc => cc.Sequence).DefaultIfEmpty(0).Max();
            Context.ChannelContents.Where(cc => cc.Content.CreatorId == userId && cc.ChannelId == timebox.ChannelId).Delete();
            selectedContents = selectedContents ?? new List<ContentPM>();
            selectedContents.ForEach(content =>
            {
                Context.ChannelContents.Add(new ChannelContent
                {
                    ChannelId = timebox.ChannelId,
                    ContentId = content.Id,
                    DelaySeconds = content.DelaySeconds,
                    Sequence = (++lastSequence),
                });
            });
            Context.SaveChanges();

            saveTime = DateTime.UtcNow.FromUtcToLocal(device.TimeZoneId).ToLongTimeString();
            var timeBoxForAdvertisementPM = timebox.GetTimeBoxForAdvertisementPM();
            timeBoxForAdvertisementPM.UsedTimeSeconds = adsTotallDuration;
            return timeBoxForAdvertisementPM;
        }

        private int ComputeChannelAdvertisementsDuration(ApplicationUser user, TimeBox timebox, IEnumerable<ContentPM> selectedContents)
        {
            var sum = Context.ChannelContents
                .Where(cc => cc.ChannelId == timebox.ChannelId && cc.Content.CreatorId != user.Id)
                .Select(cc => cc.DelaySeconds.Value)
                .DefaultIfEmpty(0)
                .Sum();
            sum += selectedContents.Sum(c => c.DelaySeconds.Value);
            return sum;
        }

        private List<EventPM> DepartEvent(EventPM e)
        {
            var departedEvents = new List<EventPM>();
            for (DateTime day = e.Start.Date; day <= e.End.Date; day = day.AddDays(1))
            {
                departedEvents.Add(new EventPM
                {
                    Start = (day == e.Start.Date ? e.Start : day),
                    End = (day == e.End.Date ? e.End : day.AddHours(24)),
                    ChannelId = e.ChannelId,
                    PlayAdvertisement = e.PlayAdvertisement
                });
            }

            departedEvents
                .Where(x => x.Start == x.End)
                .ToList()
                .ForEach(x => departedEvents.Remove(x));
            return departedEvents;
        }


        public IEnumerable<TimeBoxPM> ReadDeviceTimeBoxes(int deviceId)
        {
            return Context.TimeBoxes
                .Include(TimeBox => TimeBox.Channel)
                .Where(tb => tb.DeviceId == deviceId)
                .MapTo<TimeBoxPM>()
                .ToList();
        }

        public IEnumerable<TimeBoxForAdvertisementPM> ReadDeviceTimeBoxesForAdvertisements(int deviceId)
        {
            return Context.TimeBoxes
                .Where(tb => tb.DeviceId == deviceId)
                .Include(TimeBox => TimeBox.Channel)
                .Include(TimeBox => TimeBox.Channel.ChannelContents)
                .MapTo<TimeBoxForAdvertisementPM>()
                .ToList();
        }

        public IEnumerable<DevicePM> ReadAllDevices()
        {
            return Context.Devices
                .ToList()
                .Select(d => d.GetDevicePM());
        }

        public bool DeviceExist(string deviceIdentifier)
        {
            return Context.Devices.Any(d => d.DeviceId.ToLower() == deviceIdentifier.ToLower());
        }

        public DevicePM FindDevice(int id)
        {
            return Context.Devices.Include(d => d.Tenant).Single(d => d.Id == id).GetDevicePM();
        }

        public IEnumerable<DevicePM> FindDevices(params int[] ids)
        {
            return Context.Devices
                .Include(d => d.Tenant)
                .Where(d => ids.Contains(d.Id))
                .ToList()
                .Select(d => d.GetDevicePM());
        }

        public DevicePM FindDevice(string deviceIdentifier)
        {
            return Context.Devices
                .SingleOrDefault(d => d.DeviceId == deviceIdentifier)
                .GetDevicePM();
        }

        public void DeleteDevice(int id)
        {
            Context.Devices.Remove(Context.Devices.Find(id));
            Context.SaveChanges();
        }

        public void UpdateDevice(DevicePM devicePM)
        {
            var device = Context.Devices.Find(devicePM.Id);
            device.Name = devicePM.Name;
            device.Description = devicePM.Description;
            device.TimeZoneId = devicePM.TimeZoneId;
            device.PlayAdvertisement = devicePM.PlayAdvertisement;
            Context.SaveChanges();
        }

        public void CreateDevice(DevicePM devicePM)
        {
            var tenant = Context.Tenants.Find(devicePM.TenantId);
            if (tenant.AdvertisementStatus == TenantAdvertisementStatus.Disabled)
                devicePM.PlayAdvertisement = false;
            devicePM.RegisteredOnUtc = DateTime.UtcNow;
            Context.Devices.Add(devicePM.GetDevice());
            Context.SaveChanges();
        }

        public IEnumerable<ScheduledContentPM> ReadDeviceScheduledContents(string deviceId, string tenantSubscriptionKey)
        {
            var device = Context.Devices
                .Include(db => db.Tenant)
                .Include(db => db.TimeBoxes)
                .Single(o => o.DeviceId == deviceId);
            if (tenantSubscriptionKey.ToLower() != device.Tenant.SubscriptionKey.ToString().ToLower())
                throw new BusinessException(Resources.InvalidSubscriptionCode);
            device.LastConnectedUtc = DateTime.UtcNow;
            Context.SaveChanges();

            var timeBox = GetDeviceCurrentPlayingTimeBox(device);
            if (timeBox == null) return new List<ScheduledContentPM>();
            return Context.ChannelContents
                .Include(cc => cc.Content)
                .Where(cc => cc.ChannelId == timeBox.ChannelId && cc.Content.Flag.Value)
                .OrderBy(cc => cc.Sequence)
                .ToList()
                .Select(cc => cc.GetScheduledContentPM())
                .ToList();
        }

        public TimeBox GetDeviceCurrentPlayingTimeBox(Device device)
        {
            var clientLocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(device.TimeZoneId));
            var clientDayName = clientLocalDateTime.DayOfWeek.ToString().ToUpper();
            return device.TimeBoxes.SingleOrDefault(tb =>
                tb.WeekDay.ToUpper() == clientDayName &&
                new TimeSpan(tb.FromHour, tb.FromMinute, 0) < clientLocalDateTime.TimeOfDay &&
                clientLocalDateTime.TimeOfDay < new TimeSpan(tb.ToHour, tb.ToMinute, 0));
        }

        public DataSourceResult ReadAdvertisementDevices(IPrincipal user, DataSourceRequest request)
        {
            var userId = user.Identity.GetUserId();
            var deviceQuery = Context.Devices
                .Where(device => device.Tenant.TenantUsers.Any(tu => tu.UserId == userId) && device.PlayAdvertisement)
                .OrderByDescending(d => d.LastConnectedUtc)
                .Include(device => device.Tenant);

            var devicesCount = deviceQuery.Count();
            return deviceQuery
                .ApplyRequest(request)
                .MapTo<DeviceDetailsForDeviceListPagePM>()
                .ToList()
                .ToDataSourceResult(devicesCount);
        }
    }
}
