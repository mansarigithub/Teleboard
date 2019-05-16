using Teleboard.DomainModel.Core;
using Teleboard.Mapper.Attributes;
using Teleboard.Mapper.Profile;
using Teleboard.PresentationModel.Model.Content;

namespace Teleboard.Mapper.Core
{
    [ObjectMapper]
    public static class ContentMapper
    {
        public static void CreateMap(AutoMapperProfile profile)
        {
            profile.CreateMap<Content, ContentPM>()
                .ForMember(pm => pm.ContentTypeName, opt => opt.MapFrom(model => model.ContentType.Name));
            profile.CreateMap<ContentPM, Content>();
        }

        public static Content GetContent(this ContentPM presentationModel)
        {
            return AutoMapper.Mapper.Map<ContentPM, Content>(presentationModel);
        }

        public static ContentPM GetContentPM(this Content domainModel)
        {
            return AutoMapper.Mapper.Map<Content, ContentPM>(domainModel);
        }
    }
}

