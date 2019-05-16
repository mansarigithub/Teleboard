using System;
using Teleboard.Common.Enum;

namespace Teleboard.DomainModel.Core
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public LogType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

    }
}