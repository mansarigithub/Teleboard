using Teleboard.Mapper.Profile;

namespace Teleboard.Mapper.Initialize
{
    public static class MapperInitializer
    {
        public static void InitializModule()
        {
            AutoMapper.Mapper.Initialize(config =>
            {
                config.AddProfile<AutoMapperProfile>();
                //config.DisableConstructorMapping();
            });
        }
    }
}
