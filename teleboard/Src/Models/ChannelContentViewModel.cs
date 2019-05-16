using System.Collections.Generic;
using Teleboard.DomainModel.Core;
using Teleboard.PresentationModel.Model.Channel;
using Teleboard.PresentationModel.Model.Content;

namespace Teleboard.Models
{
    public class ChannelContentViewModel
    {
        public int DefaultDelaySeconds { get; set; }

        public ChannelPM Channel { get; set; }

        public IEnumerable<ContentType> ContentTypes { get; set; }

        public IEnumerable<ContentPM> AvailableContents { get; set; }

        public IEnumerable<ChannelContentPM> SelectedContents { get; set; }
    }
}