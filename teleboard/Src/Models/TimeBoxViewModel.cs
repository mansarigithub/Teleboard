using System.Collections.Generic;
using System.Web.Mvc;
using Teleboard.DomainModel.Core;

namespace Teleboard.Models
{
    public class TimeBoxViewModel
    {
        public TimeBox TimeBox { get; set; }

        public Channel Channel { get; set; }

        public IEnumerable<SelectListItem> SelectChannels { get; set; }

        public IEnumerable<SelectListItem> SelectWeekDays { get; set; }
    }
}