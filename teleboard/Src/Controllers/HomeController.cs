using System.Web.Mvc;
using Teleboard.Common.Enum;

namespace Teleboard.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (ApplicationUser.IsInRole(AppRole.TenantAdmin) || ApplicationUser.IsInRole(AppRole.Host))
                return RedirectToAction("Index", "Scheduler");
            else if (ApplicationUser.IsInRole(AppRole.ContentCreator) || ApplicationUser.IsInRole(AppRole.Advertiser))
                return RedirectToAction("Index", "Contents");
            else
                return RedirectToAction("Index", "Scheduler");
        }
    }
}