using System;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.PresentationModel.Model.Channel
{
    public class ChannelPM
    {
        public int Id { get; set; }

        [LocalizedName("Tenant")]
        public int TenantId { get; set; }

        [LocalizedName("Tenant")]
        public string TenantName { get; set; }

        [LocalizedName]
        [StringLengthRange(1, 50)]
        public string Name { get; set; }

        [LocalizedName]
        [StringLengthRange(1, 200)]
        public string Description { get; set; }

        public bool? Flag { get; set; }

    }
}
