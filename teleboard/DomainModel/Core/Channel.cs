using System.Collections.Generic;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    public class Channel
    {
        public int Id { get; set; }

        [LocalizedName("Tenant")]
        public int TenantId { get; set; }

        [LocalizedName]
        public string Name { get; set; }

        [LocalizedName]
        public string Description { get; set; }

        public bool? Flag { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<ChannelContent> ChannelContents { get; set; }
        public virtual ICollection<TimeBox> TimeBoxes { get; set; }
    }
}