using System;
using Teleboard.Common.Enum;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.PresentationModel.Model.Tenant
{
    public class TenantPM
    {
        public int Id { get; set; }

        public Guid SubscriptionKey { get; set; }

        [LocalizedName("Name")]
        [RequiredField]
        public string Name { get; set; }

        [LocalizedName("Description")]
        public string Description { get; set; }
        public bool? ChennalModeration { get; set; }
        public bool? ContentModeration { get; set; }
        public bool? TimeBoxModeration { get; set; }


        [RequiredField]
        [LocalizedName("TenantAdvertisementStatus")]
        public TenantAdvertisementStatus AdvertisementStatus { get; set; }


    }
}
