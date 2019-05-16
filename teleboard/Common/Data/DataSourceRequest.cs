using System.Collections.Generic;
using Teleboard.Common.Enum;

namespace Teleboard.Common.Data
{
    public class DataSourceRequest
    {
        public int? page { get; set; }
        public int? limit { get; set; }
        public string SearchValue { get; set; }
        public IEnumerable<ResourceType> IncludedResourceTypes { get; set; }
    }
}
