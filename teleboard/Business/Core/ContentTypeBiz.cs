using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Teleboard.Common.ExtensionMethod;
using Teleboard.DataAccess.Context;
using Teleboard.PresentationModel.Model.Device;
using Teleboard.Common.Data;
using System.Data.Entity;
using Teleboard.Mapper.Core;
using Teleboard.PresentationModel.Model.Log;
using Teleboard.Common.Enum;
using Teleboard.DomainModel.Core;
using Teleboard.PresentationModel.Model.ContentType;

namespace Teleboard.Business.Core
{
    public class ContentTypeBiz
    {
        private ApplicationDbContext Context { get; set; }

        public ContentTypeBiz(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumerable<ContentTypePM> ReadContentTypes()
        {
            return Context.ContentTypes
                .OrderByDescending(x => x.Name)
                .MapTo<ContentTypePM>()
                .ToList();
        }

        public ContentType ReadContentType(string mime)
        {
            return Context.ContentTypes.SingleOrDefault(o => o.Name.ToLower() == mime.ToLower());
        }

        public bool Exist(string name)
        {
            return Context.ContentTypes.Any(o => o.Name.ToLower() == name.ToLower());
        }
    }
}
