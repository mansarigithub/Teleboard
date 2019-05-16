using System.Linq;
using Teleboard.DomainModel.Core;
using Teleboard.Mapper.Attributes;
using Teleboard.Mapper.Profile;
using Teleboard.PresentationModel.Model.Tenant;

namespace Teleboard.Mapper.Core
{
    [ObjectMapper]
    public static class TenantMapper
    {
        public static void CreateMap(AutoMapperProfile profile)
        {
            profile.CreateMap<Tenant, TenantPM>();
            profile.CreateMap<TenantPM, Tenant>();
        }

        public static Tenant GetTenant(this TenantPM presentationModel)
        {
            return AutoMapper.Mapper.Map<TenantPM, Tenant>(presentationModel);
        }

        public static TenantPM GetTenantPM(this Tenant domainModel)
        {
            return AutoMapper.Mapper.Map<Tenant, TenantPM>(domainModel);
        }
    }
}

