using Teleboard.DomainModel.Core;
using Teleboard.Mapper.Attributes;
using Teleboard.Mapper.Profile;
using Teleboard.PresentationModel.Model.Content;

namespace Teleboard.Mapper.Core
{
    [ObjectMapper]
    public static class ChannelContentMapper
    {
        public static void CreateMap(AutoMapperProfile profile)
        {
            profile.CreateMap<ChannelContent, ScheduledContentPM>()
                .ForMember(pm => pm.ContentId, opt => opt.MapFrom(model => model.Content.Id))
                .ForMember(pm => pm.Duration, opt => opt.MapFrom(model => model.DelaySeconds ?? 0))
                .ForMember(pm => pm.Sequence, opt => opt.MapFrom(model => model.Sequence))
                .ForMember(pm => pm.Description, opt => opt.MapFrom(model => model.Content.Description))
                .ForMember(pm => pm.ContentSource, opt => opt.MapFrom(model => model.Content.Source))
                .ForMember(pm => pm.ContentTenantId, opt => opt.MapFrom(model => model.Content.TenantId));

            profile.CreateMap<ChannelContent, ChannelContentPM>()
                .ForMember(pm => pm.DelaySeconds, opt => opt.MapFrom(model => model.DelaySeconds))
                .ForMember(pm => pm.ContentTypeId, opt => opt.MapFrom(model => model.Content.ContentTypeId))
                .ForMember(pm => pm.ContentTypeName, opt => opt.MapFrom(model => model.Content.ContentType.Name))
                .ForMember(pm => pm.FileSize, opt => opt.MapFrom(model => model.Content.FileSize))
                .ForMember(pm => pm.Flag, opt => opt.MapFrom(model => model.Content.Flag))
                .ForMember(pm => pm.Guid, opt => opt.MapFrom(model => model.Content.Guid))
                .ForMember(pm => pm.Source, opt => opt.MapFrom(model => model.Content.Source))
                .ForMember(pm => pm.TenantId, opt => opt.MapFrom(model => model.Content.TenantId))
                .ForMember(pm => pm.TenantName, opt => opt.MapFrom(model => model.Content.Tenant.Name))
                .ForMember(pm => pm.ContentDuration, opt => opt.MapFrom(model => model.Content.Duration))
                .ForMember(pm => pm.ContentDescription, opt => opt.MapFrom(model => model.Content.Description));

            profile.CreateMap<ChannelContentPM, ContentPM>()
                .ForMember(contentPM => contentPM.Id, opt => opt.MapFrom(channelContentPM => channelContentPM.ContentId))
                .ForMember(contentPM => contentPM.Duration, opt => opt.MapFrom(channelContentPM => channelContentPM.ContentDuration))
                .ForMember(contentPM => contentPM.Description, opt => opt.MapFrom(channelContentPM => channelContentPM.ContentDescription));

        }

        public static ScheduledContentPM GetScheduledContentPM(this ChannelContent domainModel)
        {
            return AutoMapper.Mapper.Map<ChannelContent, ScheduledContentPM>(domainModel);
        }

        public static ChannelContentPM GetChannelContentPM(this ChannelContent domainModel)
        {
            return AutoMapper.Mapper.Map<ChannelContent, ChannelContentPM>(domainModel);
        }

        public static ContentPM GetContentPM(this ChannelContentPM domainModel)
        {
            return AutoMapper.Mapper.Map<ChannelContentPM, ContentPM>(domainModel);
        }
    }
}