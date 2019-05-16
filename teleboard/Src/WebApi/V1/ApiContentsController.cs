using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using Teleboard.Business.Core;
using Teleboard.Common.Configuration;
using Teleboard.Common.Enum;
using Teleboard.Common.ExtensionMethod;
using Teleboard.Common.Media;
using Teleboard.PresentationModel.Model.Content;
using Teleboard.UI.WebApi.V1.Infrastructure;

namespace Teleboard.UI.WebApi.V1
{
    public class ContentsController : ApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/v1/contents/{contentSource}")]
        public HttpResponseMessage Get(string contentSource)
        {
            bool authorized = true;
            ContentPM content;
            try
            {
                content = ContentBiz.ReadContent(contentSource);

                var contentType = MediaTypeHeaderValue.Parse(content.ContentTypeName.ToLower());
                var fileFullName = ContentBiz.ComputeContentFilePath(content.TenantId, content.Source);
                var fileStram = File.OpenRead(fileFullName);
                if (Request.Headers.Range != null)
                {
                    HttpResponseMessage partialResponse = Request.CreateResponse(HttpStatusCode.PartialContent);
                    partialResponse.Content = new ByteRangeStreamContent(fileStram, Request.Headers.Range, contentType);
                    return partialResponse;
                }

                HttpResponseMessage fullResponse = Request.CreateResponse(HttpStatusCode.OK);
                fullResponse.Content = new StreamContent(fileStram);
                fullResponse.Content.Headers.ContentType = contentType;
                return fullResponse;
            }
            catch (InvalidByteRangeException invalidByteRangeException)
            {
                return Request.CreateErrorResponse(invalidByteRangeException);
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
            }

            //if (User.Identity.IsAuthenticated) {
            //    if (User.IsInRole("Host")) {
            //        authorized = true;
            //    }
            //    else {
            //        int[] tenantIds = db.GetTenantsFromUser(User.Identity.GetUserId(), User.IsInRole("Host"));
            //        if (tenantIds.Any(o => o == id)) {
            //            authorized = true;
            //        }
            //    }
            //}
            //else {
            //    authorized = Request.Headers["subscription-key"] != null && Request.Headers["subscription-key"].ToLower() == tenantConent.Tenant.SubscriptionKey.ToString().ToLower();
            //}
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/v1/contents/thumbnail/{contentSource}")]
        public HttpResponseMessage GetThumbnail(string contentSource)
        {
            ContentPM content;
            try
            {
                content = ContentBiz.ReadContent(contentSource);
                if (content == null) throw new Exception();
                var contentType = MediaTypeHeaderValue.Parse(content.ContentTypeName.ToLower());
                var fileFullName = ContentBiz.ComputeContentFilePath(content.TenantId, content.Source);
                var thumbFullName = ContentBiz.ComputeContentThumbnailFilePath(content.TenantId, content.Source);
                if (!File.Exists(thumbFullName))
                    MediaHelper.CreateThumbnail(fileFullName, thumbFullName, content.ResourceType);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StreamContent(File.OpenRead(thumbFullName));
                response.Content.Headers.ContentType = contentType;
                return response;
            }
            catch (Exception exp)
            {
                LogBiz.CreateLog(LogType.Error, exp.ToString(), exp.Message);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString());
            }
        }

        [HttpPut]
        public async Task<IHttpActionResult> PutContent(Guid id)
        {
            var contentGuid = id;
            await ContentBiz.UpdateContentAsync(contentGuid, GetHeader("new-description"));
            return ApiResult(new { });
        }

    }
}