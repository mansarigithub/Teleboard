using System.Web.Mvc;
using Teleboard.Business.Core;
using Teleboard.Common.Data;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host")]
    public class LogController : BaseController
    {
        public LogController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ReadLogs(DataSourceRequest request)
        {
            return Json(LogBiz.ReadLogs(request), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            LogBiz.DeleteLog(id);
            return RedirectToAction("Index");
        }
    }
}
