using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Http;
using Teleboard.Business.Core;
using Teleboard.DataAccess.Context;
using Microsoft.AspNet.Identity;
using System.Linq;
using System;
using Teleboard.Common.Exception;
using Teleboard.UI.WebApi.V1.Infrastructure;
using System.Net;
using Teleboard.UI.WebApi.V1.DataContract;
using Teleboard.DomainModel.Core;

namespace Teleboard.UI.WebApi
{
    [ApiExceptionFilterAttribute]
    [ApiAuthenticateAttribute]
    public class ApiBaseController : ApiController
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
        private AuthenticationTokenBiz _authenticationTokenBiz;

        public ApplicationDbContext AppDbContext
        {
            get
            {
                return System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public new ApplicationUser User
        {
            get
            {
                return base.User as ApplicationUser;
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

        public AuthenticationTokenBiz AuthenticationTokenBiz
        {
            get
            {
                if (_authenticationTokenBiz == null)
                    _authenticationTokenBiz = new AuthenticationTokenBiz(AppDbContext);
                return _authenticationTokenBiz;
            }
        }

        #endregion

        public string GetHeader(string key, bool required = true)
        {
            var headers = Request.Headers.Where(h => h.Key == key && h.Value.Any());
            if (headers.Count() >= 1) {
                var value = headers.First().Value.FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            if (required)
                throw new HttpParameterNotFountException() { ParameterName = key };
            else
                return null;
        }

        public ApiResult ApiResult(ApiResponseCode code, HttpStatusCode httpStatus, string description, object data)
        {
            return new ApiResult(Request, code, httpStatus, description, data);
        }

        protected ApiResult ApiResult(object data)
        {
            return new ApiResult(Request, data);
        }

    }
}