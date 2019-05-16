using System;
using Teleboard.Validation.Attribute;

namespace Teleboard.Models
{
    public class ContentEditViewModel
    {
        public int Id { get; set; }

        public string ContentType { get; set; }

        public int TenantId { get; set; }

        [RequiredField]
        public string Source { get; set; }

        [StringLengthRange(1, 200)]
        public string Description { get; set; }

        public bool? Flag { get; set; }
        public Guid Guid { get; set; }
    }
}