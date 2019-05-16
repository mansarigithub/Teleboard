using System;
using System.Linq;
using Teleboard.Common.Enum;
using Teleboard.DomainModel.Core;
using Teleboard.Mapper.Attributes;
using Teleboard.Mapper.Profile;
using Teleboard.PresentationModel.Model.Device;

namespace Teleboard.Mapper.Core
{
    [ObjectMapper]
    public static class DeviceMapper
    {
        public static void CreateMap(AutoMapperProfile profile)
        {
            profile.CreateMap<Device, DevicePM>()
                .ForMember(pm => pm.TenantName, opt => opt.MapFrom(model => model.Tenant.Name))
                .ForMember(pm => pm.ConnectionTypeName, opt => opt.MapFrom(model => model.ConnectionType.Name))
                .ForMember(pm => pm.IsAdvertisementsEnabled, opt => opt.MapFrom(model => model.Tenant.AdvertisementStatus == TenantAdvertisementStatus.Enabled));

            profile.CreateMap<DevicePM, Device>()
                .ForMember(m => m.DeviceId, opt => opt.MapFrom(pm => pm.DeviceId.Trim().ToLower()));

            profile.CreateMap<Device, DeviceDetailsForDeviceListPagePM>()
                .ForMember(pm => pm.TenantName, opt => opt.MapFrom(model => model.Tenant.Name))
                .ForMember(pm => pm.TenantId, opt => opt.MapFrom(model => model.Tenant.Id))
                .ForMember(pm => pm.LastConnectedLocal, opt => opt.Ignore());

        }

        public static Device GetDevice(this DevicePM presentationModel)
        {
            return AutoMapper.Mapper.Map<DevicePM, Device>(presentationModel);
        }

        public static DevicePM GetDevicePM(this Device domainModel)
        {
            return AutoMapper.Mapper.Map<Device, DevicePM>(domainModel);
        }
    }
}

