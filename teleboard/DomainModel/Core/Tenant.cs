using System;
using System.Collections.Generic;
using Teleboard.Common.Enum;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    public class Tenant
    {
        public int Id { get; set; }

        public Guid SubscriptionKey { get; set; }

        [RequiredField]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool? ChennalModeration { get; set; }
        public bool? ContentModeration { get; set; }
        public bool? TimeBoxModeration { get; set; }
        public TenantAdvertisementStatus AdvertisementStatus { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Channel> Channels { get; set; }
        public virtual ICollection<Content> Contents { get; set; }
        public virtual ICollection<TenantUser> TenantUsers { get; set; }
   }
}