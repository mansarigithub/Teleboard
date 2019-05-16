using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Teleboard.Attributes;

namespace Teleboard.Models
{
    public class DeviceChannelViewModel
    {
        public int Id { get; set; }    
        public string DeviceName { get; set; }
       
        public int DeviceId { get; set; }

        public string ChannelName { get; set; }

        public int ChannelId { get; set; }

        public int TenantId { get; set; }

        public string TenantName { get; set; }


    }
}