using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;

namespace Teleboard.Models.Mapping
{
    public class TenantUserMap : EntityTypeConfiguration<TenantUser>
    {
        public TenantUserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("TenantUser");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.TenantUsers)
                .HasForeignKey(d => d.TenantId);

            this.HasRequired(t => t.User)
                .WithMany(t => t.TenantUsers)
                .HasForeignKey(d => d.UserId);
        }
    }
}
