using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Teleboard.Startup))]
namespace Teleboard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
