using Microsoft.AspNet.Identity;
using System.Linq;
using System.Security.Principal;
using Teleboard.Common.ExtensionMethod;
using Teleboard.DataAccess.Context;
using Teleboard.Common.Data;
using System.Data.Entity;
using Teleboard.PresentationModel.Model.Channel;
using System;
using System.Collections.Generic;
using Teleboard.Mapper.Core;
using System.Threading.Tasks;
using Teleboard.DomainModel.Core;

namespace Teleboard.Business.Core
{
    public class ChannelBiz : BizBase<Channel>
    {
        private ApplicationDbContext Context { get; set; }

        public ChannelBiz(ApplicationDbContext context) : base(context)
        {
            Context = context;
        }

        public DataSourceResult ReadAllChannels(DataSourceRequest request)
        {
            return Context.Channels
                .Include(device => device.Tenant)
                .MapTo<ChannelPM>()
                .ToDataSourceResult(request);
        }

        public IEnumerable<ChannelPM> ReadChannelsForDeviceSchedule(int deviceId)
        {
            return Context.Channels.Where(ch => ch.Tenant.Devices.Any(d => d.Id == deviceId))
                .MapTo<ChannelPM>()
                .ToList();
        }

        public DataSourceResult ReadChannelsForTenantAdmin(IPrincipal user, DataSourceRequest request)
        {
            var userId = user.Identity.GetUserId();
            return Context.Channels
                 .Include(device => device.Tenant)
                 .Where(channel => channel.Tenant.TenantUsers.Any(x => x.UserId == userId))
                 .MapTo<ChannelPM>()
                 .ToDataSourceResult(request);
        }

        public ChannelPM FindChannel(int id)
        {
            return Context.Channels.Find(id).GetChannelPM();
        }

        public async Task AddContentToChannelAsync(int channelId, int contentId, int? delay)
        {
            if (!await Context.ChannelContents.AnyAsync(o => o.ChannelId == channelId && o.ContentId == contentId)) {
                await Context.ChannelContents
                    .Where(o => o.ChannelId == channelId)
                    .ForEachAsync(o => o.Sequence = o.Sequence + 1);

                Context.ChannelContents.Add(new ChannelContent {
                    ChannelId = channelId,
                    ContentId = contentId,
                    DelaySeconds = delay ?? 10,
                    Sequence = 1
                });
                await Context.SaveChangesAsync();
            }

        }

        public async Task SwapChannelContentsSequence(int channelId, int firstContentId, int secondContentId)
        {
            var channelContents = await Context.ChannelContents
                .Where(o => o.ChannelId == channelId && (o.ContentId == firstContentId || o.ContentId == secondContentId)).ToListAsync();
            if (channelContents == null || channelContents.Count != 2)
                throw new InvalidOperationException();

            int tmp = channelContents[0].Sequence;
            channelContents[0].Sequence = channelContents[1].Sequence;
            channelContents[1].Sequence = tmp;
            await Context.SaveChangesAsync();
        }

        public ChannelPM ReadChannel(int id)
        {
            return Context.Channels
                .Include(ch => ch.Tenant)
                .Where(ch => ch.Id == id)
                .MapTo<ChannelPM>()
                .Single();
        }
    }
}
