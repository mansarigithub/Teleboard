using Teleboard.DomainModel.Core;
using Teleboard.Mapper.Attributes;
using Teleboard.Mapper.Profile;
using Teleboard.PresentationModel.Model.ContentType;

namespace Teleboard.Mapper.Core
{
    [ObjectMapper]
    public static class ContentTypeMapper
    {
        public static void CreateMap(AutoMapperProfile profile)
        {
            profile.CreateMap<ContentType, ContentTypePM>();
            profile.CreateMap<ContentTypePM, ContentType>();
        }

        public static ContentType GetContentType(this ContentTypePM presentationModel)
        {
            return AutoMapper.Mapper.Map<ContentTypePM, ContentType>(presentationModel);
        }

        public static ContentTypePM GetContentTypePM(this ContentType domainModel)
        {
            return AutoMapper.Mapper.Map<ContentType, ContentTypePM>(domainModel);
        }
    }
}

