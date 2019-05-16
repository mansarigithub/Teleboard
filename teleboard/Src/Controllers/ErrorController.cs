using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Teleboard.Models;

namespace Teleboard.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();
            return View("Error",new HandleErrorInfo(exception,"",""));
        }

        public ActionResult Page([FromUri]ErrorPageModel errorPageModel)
        {
            ViewBag.Trace = this.TempData["trace"];
            return View("ErrorPage", errorPageModel);
        }

    }
}