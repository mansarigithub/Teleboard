using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Teleboard.Business.Core;
using Teleboard.Common.Enum;
using Teleboard.Common.Exception;
using Teleboard.DataAccess.Context;
using Teleboard.DomainModel.Core;
using Teleboard.UI.Infrastructure.Mvc;
using System.Web.Routing;
using Teleboard.UI.Infrastructure.Globalization;

namespace Teleboard.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private LogBiz _logBiz;
        private DeviceBiz _deviceBiz;
        private ChannelBiz _channelBiz;
        private ContentBiz _contentBiz;
        private TenantBiz _tenantBiz;
        private ContentTypeBiz _contentTypeBiz;
        private ApplicationUserBiz _applicationUserBiz;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ApplicationDbContext AppDbContext
        {
            get
            {
                return System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            }
        }

        protected ApplicationUser ApplicationUser
        {
            get
            {
                return HttpContext.User as ApplicationUser;
            }
        }

        #region Biz 
        public LogBiz LogBiz
        {
            get
            {
                if (_logBiz == null)
                    _logBiz = new LogBiz(AppDbContext);
                return _logBiz;
            }
        }

        public DeviceBiz DeviceBiz
        {
            get
            {
                if (_deviceBiz == null)
                    _deviceBiz = new DeviceBiz(AppDbContext);
                return _deviceBiz;
            }
        }

        public ChannelBiz ChannelBiz
        {
            get
            {
                if (_channelBiz == null)
                    _channelBiz = new ChannelBiz(AppDbContext);
                return _channelBiz;
            }
        }

        public ContentBiz ContentBiz
        {
            get
            {
                if (_contentBiz == null)
                    _contentBiz = new ContentBiz(AppDbContext);
                return _contentBiz;
            }
        }

        public TenantBiz TenantBiz
        {
            get
            {
                if (_tenantBiz == null)
                    _tenantBiz = new TenantBiz(AppDbContext);
                return _tenantBiz;
            }
        }

        public ContentTypeBiz ContentTypeBiz
        {
            get
            {
                if (_contentTypeBiz == null)
                    _contentTypeBiz = new ContentTypeBiz(AppDbContext);
                return _contentTypeBiz;
            }
        }

        public ApplicationUserBiz ApplicationUserBiz
        {
            get
            {
                if (_applicationUserBiz == null)
                    _applicationUserBiz = new ApplicationUserBiz(AppDbContext);
                return _applicationUserBiz;
            }
        }

        #endregion

        #region User Identity
        public string UserId
        {
            get
            {
                return ApplicationUser.Identity.GetUserId();
            }
        }

        #endregion

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is BusinessException)
                HandleBussinessException(filterContext);
            else
                HandleUnhandledException(filterContext);
        }

        private void HandleUnhandledException(ExceptionContext filterContext)
        {
            var logBiz = new LogBiz(AppDbContext);
            var title = filterContext?.RequestContext?.HttpContext?.Request?.Url?.ToString();
            var description = string.Format("Exception: {0}{1}Inner Exception: {2}",
                filterContext?.Exception.Message, Environment.NewLine, filterContext?.Exception?.InnerException?.Message);
            logBiz.CreateLog(LogType.Error, title, description);

            if (!filterContext.HttpContext.Request.IsLocal)
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.HttpContext.Response.Clear();
                filterContext.ExceptionHandled = true;

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = Json(new
                    {
                        Status = "Error",
                    });
                }
                else
                {
                    filterContext.Result = View("_Error");
                }
            }
        }

        private void HandleBussinessException(ExceptionContext filterContext)
        {
            var bizException = filterContext.Exception as BusinessException;
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            filterContext.HttpContext.Response.Clear();
            filterContext.ExceptionHandled = true;

            filterContext.Result = Json(new
            {
                Status = "BizError",
                Message = bizException.Message,
                Type = bizException.Type.ToString(),
                Code = (int)bizException.Type,
            });
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            if (!user.Identity.IsAuthenticated) return;

            var applicationUser = SessionManager.GetUserIdentity();
            applicationUser = applicationUser ?? UserManager.FindByName(user.Identity.Name);

            applicationUser.Identity = user.Identity;
            filterContext.HttpContext.User = applicationUser;
            SessionManager.StoreUserIdentity(applicationUser);

            base.OnAuthentication(filterContext);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(ApplicationUser != null)
            {
                AppLanguage.SetLanguage(ApplicationUser.GetCulture());
            }
        }
    }
}