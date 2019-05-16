using System.Reflection;
using Teleboard.Common.ExtensionMethod;
using Teleboard.Mapper.Attributes;

namespace Teleboard.Mapper.Profile
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .ForEach(type => {
                    var attribute = type.GetCustomAttribute<ObjectMapperAttribute>();
                    if (attribute != null) {
                        MethodInfo methodInfo = type.GetMethod("CreateMap", BindingFlags.Static | BindingFlags.Public);
                        methodInfo.Invoke(null, new object[] { this });
                    }
                });
        }

        public override string ProfileName
        {
            get { return this.GetType().Name; }
        }
    }

}
