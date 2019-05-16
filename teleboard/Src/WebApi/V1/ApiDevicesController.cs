using System;
using System.Collections.Generic;
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
using Teleboard.Common.Exception;
using Teleboard.Common.Media;
using Teleboard.PresentationModel.Model.Content;
using Teleboard.PresentationModel.Model.Device;
using Teleboard.UI.WebApi.V1.Infrastructure;

namespace Teleboard.UI.WebApi.V1
{
    public class DevicesController : ApiBaseController
    {
        [HttpGet]
        public async Task<IHttpActionResult> GetDevices()
        {
            const string HEADER = "tenant-id";
            string tenantIdHeader = GetHeader(HEADER, required: false);
            if (tenantIdHeader == null) {
                if (User.IsHostAdmin)
                    return ApiResult(MapDevices(DeviceBiz.ReadAllDevices()));
                else
                    throw new HttpParameterNotFountException(HEADER);
            }

            int tenantId;
            if (int.TryParse(tenantIdHeader, out tenantId)) {
                if (User.IsHostAdmin || await ApplicationUserBiz.UserHasMembershipInTenantAsync(User.Id, tenantId))
                    return ApiResult(MapDevices(DeviceBiz.ReadTenantDevices(tenantId)));
                else
                    throw new UnauthorizedAccessException();
            }
            throw new HttpParameterNotFountException();
        }


        public IEnumerable<object> MapDevices(IEnumerable<DevicePM> devices)
        {
            return devices.Select(d => new {
                d.Id,
                d.Name,
                d.Description,
                d.DeviceId,
                d.LastConnectedUtc,
                d.LastConnectedUtcString,
                d.LastConnectedLocal,
                d.LastConnectedLocalString
            });
        }
    }
}