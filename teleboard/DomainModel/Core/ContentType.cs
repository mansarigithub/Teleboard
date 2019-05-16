using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Teleboard.Common.Enum;
using Teleboard.Common.Media;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    public class ContentType
    {
        public int Id { get; set; }

        [RequiredField]
        public string Name { get; set; }

        public virtual ICollection<Content> Contents { get; set; }

        [NotMapped]
        public ResourceType ResourceType
        {
            get
            {
                return MediaHelper.GetResourceType(Name);
            }
        }
    }
}