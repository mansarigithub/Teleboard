namespace Teleboard.DomainModel.Core
{
    public class ChannelContent
    {
        public int Id { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public int Sequence { get; set; }

        public int? DelaySeconds { get; set; }

        public virtual Channel Channel { get; set; }

        public virtual Content Content { get; set; }
    }
}