using System.Linq;
using Teleboard.DomainModel.Core;
using Teleboard.Mapper.Attributes;
using Teleboard.Mapper.Profile;
using Teleboard.PresentationModel.Model.Channel;

namespace Teleboard.Mapper.Core
{
    [ObjectMapper]
    public static class ChannelMapper
    {
        public static void CreateMap(AutoMapperProfile profile)
        {
            profile.CreateMap<Channel, ChannelPM>()
                .ForMember(pm => pm.TenantName, opt => opt.MapFrom(model => model.Tenant.Name));
            profile.CreateMap<ChannelPM, Channel>();
        }

        public static Channel GetChannel(this ChannelPM presentationModel)
        {
            return AutoMapper.Mapper.Map<ChannelPM, Channel>(presentationModel);
        }

        public static ChannelPM GetChannelPM(this Channel domainModel)
        {
            return AutoMapper.Mapper.Map<Channel, ChannelPM>(domainModel);
        }
    }
}

