using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;

namespace Teleboard.Models.Mapping
{
    public class ConnectionTypeMap : EntityTypeConfiguration<ConnectionType>
    {
        public ConnectionTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            //this.ToTable("Device");

            // Relationships
        }
    }
}
