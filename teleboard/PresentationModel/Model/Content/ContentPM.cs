using System;
using Teleboard.Common.Enum;
using Teleboard.Common.ExtensionMethod;
using Teleboard.Common.Media;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.PresentationModel.Model.Content
{
    public class ContentPM
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public string TenantName { get; set; }

        public int ContentTypeId { get; set; }

        public string Source { get; set; }

        [LocalizedName]
        [StringLengthRange(1, 200)]
        [RequiredField]
        public string Description { get; set; }

        public bool? Flag { get; set; }

        public int? FileSize { get; set; }

        public Guid? Guid { get; set; }

        public string ContentTypeName { get; set; }

        public int? DelaySeconds { get; set; }
        public int? Duration { get; set; }

        public ResourceType ResourceType
        {
            get
            {
                return MediaHelper.GetResourceType(ContentTypeName);
            }
        }

        public string ResourceTypeName
        {
            get
            {
                return ResourceType.ToString();
            }
        }

        [LocalizedName("Size")]
        public string FileSizeString
        {
            get
            {
                return FileSize.HasValue ? FileSize.Value.ToFileSizeString() : "";
            }
        }

        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
