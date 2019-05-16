using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Teleboard.Business.Core;
using Teleboard.Localization;
using Teleboard.UI.WebApi.V1.DataContract;
using Teleboard.UI.WebApi.V1.Infrastructure;

namespace Teleboard.UI.WebApi.V1
{
    [RoutePrefix("api/v1/auth")]
    public class AuthenticationController : ApiBaseController
    {
        [Route("signin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> signin()
        {
            var user = await UserManager.FindByNameAsync(GetHeader("username"));
            if (user == null || !await UserManager.CheckPasswordAsync(user, GetHeader("password"))) {
                return ApiResult(
                    ApiResponseCode.AuthenticationFailed,
                    HttpStatusCode.Unauthorized,
                    Resources.InvalidUserNameOrPassword,
                    data: null);
            }

            if (!user.EmailConfirmed) {
                return ApiResult(
                    ApiResponseCode.EmailNotConfirmed,
                    HttpStatusCode.Unauthorized,
                    Resources.UserEmailIsNotConfirmed,
                    data: null);
            }

            var token = AuthenticationTokenBiz.GenerateToken(user.Id);
            return ApiResult(new { authToken = token });
        }
    }
}