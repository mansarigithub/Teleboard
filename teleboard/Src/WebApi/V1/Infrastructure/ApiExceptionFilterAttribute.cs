using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Teleboard.Common.Exception;
using Teleboard.Localization;
using Teleboard.UI.WebApi.V1.DataContract;

namespace Teleboard.UI.WebApi.V1.Infrastructure
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var exp = context.Exception;
            HttpStatusCode httpStatusCode;
            ApiResponseCode apiCode;
            string description = "";

            if (exp is HttpParameterNotFountException) {
                httpStatusCode = HttpStatusCode.BadRequest;
                apiCode = ApiResponseCode.ParameterNotAvailable;
                description = $"{Resources.MissingParameter}: {(exp as HttpParameterNotFountException).ParameterName}";
            }
            else if (exp is ResourceNotFountException) {
                httpStatusCode = HttpStatusCode.NotFound;
                apiCode = ApiResponseCode.ResourceNotFount;
                description = exp.Message;
            }
            else if (exp is UnauthorizedAccessException) {
                httpStatusCode = HttpStatusCode.Unauthorized;
                apiCode = ApiResponseCode.Unauthorized;
                description = ApiResponseCode.Unauthorized.ToString();
            }
            else {
                httpStatusCode = HttpStatusCode.InternalServerError;
                apiCode = ApiResponseCode.UnknownError;
                description = ApiResponseCode.UnknownError.ToString();
            }

            context.Response = context.Request.CreateResponse(httpStatusCode, new ApiResponseData {
                code = (int)apiCode,
                description = description,
                data = new {}
            });
        }
    }
}