using System;
using Teleboard.Common.Enum;

namespace Teleboard.PresentationModel.Model.Content
{
    public class ChannelContentPM
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public int ContentId { get; set; }
        public int ChannelId { get; set; }

        public int? DelaySeconds { get; set; }
        public int? ContentDuration { get; set; }
        public string ContentDescription { get; set; }
        public string TenantName { get; set; }

        public int ContentTypeId { get; set; }

        public string Source { get; set; }

        public string Description { get; set; }

        public bool? Flag { get; set; }

        public int? FileSize { get; set; }

        public Guid? Guid { get; set; }

        public string ContentTypeName { get; set; }
        public ResourceType ResourceType
        {
            get
            {
                if (ContentTypeName.StartsWith("video"))
                    return ResourceType.Video;
                else if (ContentTypeName.StartsWith("image"))
                    return ResourceType.Image;
                else
                    return ResourceType.Unknown;
            }
        }

        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
