using System;
using System.Linq;
using Teleboard.DomainModel.Core;
using Teleboard.Mapper.Attributes;
using Teleboard.Mapper.Profile;
using Teleboard.PresentationModel.Model.Device;

namespace Teleboard.Mapper.Core
{
    [ObjectMapper]
    public static class TimeBoxMapper
    {
        public static void CreateMap(AutoMapperProfile profile)
        {
            profile.CreateMap<TimeBox, TimeBoxPM>()
                .ForMember(pm => pm.ChannelName, opt => opt.MapFrom(model => model.Channel.Name))
                .ForMember(pm => pm.Duration, opt => opt.Ignore());

            profile.CreateMap<TimeBox, TimeBoxForAdvertisementPM>()
                .ForMember(pm => pm.ChannelName, opt => opt.MapFrom(model => model.Channel.Name))
                .ForMember(pm => pm.UsedTimeSeconds, opt => opt.MapFrom(model => model.Channel.ChannelContents.Sum(cc => cc.DelaySeconds)))
                .ForMember(pm => pm.Duration, opt => opt.Ignore());


            //profile.CreateMap<TimeBoxPM, TimeBox>()
            //    .ForMember(m => m.TimeBoxId, opt => opt.MapFrom(pm => pm.TimeBoxId.Trim().ToLower()));
        }

        public static TimeBox GetTimeBox(this TimeBoxPM presentationModel)
        {
            return AutoMapper.Mapper.Map<TimeBoxPM, TimeBox>(presentationModel);
        }

        public static TimeBoxForAdvertisementPM GetTimeBoxForAdvertisementPM(this TimeBox domainModel)
        {
            return AutoMapper.Mapper.Map<TimeBox, TimeBoxForAdvertisementPM>(domainModel);
        }
    }
}

