namespace Teleboard.PresentationModel.Model.Content
{
    public class ScheduledContentPM
    {
        public int ContentId { get; set; }

        public int Duration { get; set; }

        public int Sequence { get; set; }

        public string Description { get; set; }

        public string ContentSource { get; set; }
        public int ContentTenantId { get; set; }

        public string Url { get; set; }

        //public string ThumbnailUrl { get; set; }
    }
}
