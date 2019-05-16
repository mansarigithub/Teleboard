using System;

namespace Teleboard.Models.PresentationModel.Model.TimeBox
{
    public class EventPM
    {
        public string Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int ChannelId { get; set; }
        public bool PlayAdvertisement { get; set; }


    }
}