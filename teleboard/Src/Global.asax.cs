using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Teleboard.UI;

namespace Teleboard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            //AA vasl sho db va dictionary basaaz
            // lazy loading avalesh nulleh agar bood q bezan agar na bekhoon

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            ApplicationInitializer.Initialize();
        }

        protected void Application_BeginRequest()
        {
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.Flush();
            }
        }
    }
}
