using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    public class ConnectionType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [RequiredField]
        public string Name { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
    }
}