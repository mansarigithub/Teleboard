using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Teleboard.Common.Exception;
using Teleboard.UI.WebApi.V1.DataContract;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Teleboard.Business.Core;
using Teleboard.DataAccess.Context;
using System.Collections;
using System.Collections.Generic;
using Teleboard.Localization;
using System.Web.Http;

namespace Teleboard.UI.WebApi.V1.Infrastructure
{
    public class ApiAuthenticateAttribute : Attribute, System.Web.Http.Filters.IAuthenticationFilter
    {
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var allowAttr = context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().FirstOrDefault();
            if (allowAttr != null) return;

            var headers = context.Request.Headers.Where(h => h.Key == "auth-token" && h.Value.Any());
            if (headers.Count() != 1) {
                Create401Response(context);
                return;
            }
            var authKey = headers.Single().Value.First();
            if (string.IsNullOrWhiteSpace(authKey)) {
                Create401Response(context);
                return;
            }

            var userBiz = new ApplicationUserBiz(new ApplicationDbContext());
            var user = await userBiz.FindUserByTokenAsync(authKey);
            if (user == null) {
                Create401Response(context);
                return;
            }
            context.Principal = user;
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {

        }

        public void Create401Response(HttpAuthenticationContext context)
        {
            context.ErrorResult = new ApiResult(context.Request, ApiResponseCode.AuthenticationFailed, HttpStatusCode.Unauthorized,
                SysResource.Unauthorized, new { });
        }
    }
}