using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Teleboard.UI.WebApi.V1.DataContract;

namespace Teleboard.UI.WebApi.V1.Infrastructure
{
    public class ApiResult : IHttpActionResult
    {
        ApiResponseData _responseData;
        HttpRequestMessage _request;
        HttpStatusCode _httpStatus;


        public ApiResult(HttpRequestMessage request, ApiResponseCode code, HttpStatusCode httpStatus, string description, object data)
        {
            _responseData = new ApiResponseData() {
                code = (int)code,
                description = description,
                data = data
            };
            _httpStatus = httpStatus;
            _request = request;
        }

        public ApiResult(HttpRequestMessage request, object data)
        {
            _responseData = new ApiResponseData() {
                code = (int)ApiResponseCode.OK,
                description = ApiResponseCode.OK.ToString(),
                data = data
            };
            _httpStatus = HttpStatusCode.OK;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_httpStatus) {
                Content = new StringContent(JsonConvert.SerializeObject(_responseData)),
                RequestMessage = _request,
            };
            return Task.FromResult(response);
        }
    }
}