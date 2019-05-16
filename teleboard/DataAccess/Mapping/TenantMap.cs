using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;

namespace Teleboard.Models.Mapping
{
    public class TenantMap : EntityTypeConfiguration<Tenant>
    {
        public TenantMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Description).HasMaxLength(200);


            // Table & Column Mappings
            this.ToTable("Tenant");

            // Relationships

        }
    }
}
