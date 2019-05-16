using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Teleboard.Attributes;

namespace Teleboard.Models
{
    public class DeviceViewModel
    {
        public int Id { get; set; }

        [LocalizedDisplayName(Name: "Tenant")]
        public string TenantName { get; set; }

        [LocalizedDisplayName(Name: "Name")]
        public string Name { get; set; }
        [LocalizedDisplayName(Name: "DeviceId")]
        public string DeviceId { get; set; }

        [LocalizedDisplayName(Name: "Description")]
        public string Description { get; set; }

        [DisplayFormat(NullDisplayText = "-")]
        public int? Latitude { get; set; }

        [DisplayFormat(NullDisplayText = "-")]
        public int? Longitude { get; set; }

        [DisplayFormat(NullDisplayText = "-")]
        [LocalizedDisplayName(Name: "Version")]
        public string Version { get; set; }

        [DisplayFormat(NullDisplayText = "-")]
        [LocalizedDisplayName(Name: "Connection Type")]
        public string ConnectionType { get; set; }

        [DisplayFormat(NullDisplayText = "-")]
        [LocalizedDisplayName(Name: "Last Connected Date")]
        public DateTime? LastConnectedUtc { get; set; }

        [DisplayFormat(NullDisplayText = "-")]
        [LocalizedDisplayName(Name: "Registered Date")]
        public DateTime? RegisteredOnUtc { get; set; }
        

    }
}