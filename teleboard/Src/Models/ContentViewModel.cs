using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teleboard.Attributes;

namespace Teleboard.Models
{
    public class ContentViewModel
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        [LocalizedDisplayName(Name: "Tenant Name")]
        public string TenantName { get; set; }

        [LocalizedDisplayName(Name: "Content Type")]
        public string ContentType { get; set; }

        [LocalizedDisplayName(Name: "Source")]
        public string Source { get; set; }

        [LocalizedDisplayName(Name: "Description")]
        public string Description { get; set; }

        public bool? Flag { get; set; }
        public int? FileSize { get; set; }
        public Guid? Guid { get; set; }
    }
}