using System;
using System.Collections.Generic;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    public class Content
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public int ContentTypeId { get; set; }

        [RequiredField]
        public string Source { get; set; }

        public string Description { get; set; }

        public bool? Flag { get; set; }

        public int? FileSize { get; set; }

        public Guid? Guid { get; set; }
        public int? Duration { get; set; }

        public string CreatorId { get; set; }


        public virtual Tenant Tenant { get; set; }
        public virtual ContentType ContentType { get; set; }
        public virtual ICollection<ChannelContent> ChannelContents { get; set; }
        public virtual ApplicationUser Creator { get; set; }
    }
}