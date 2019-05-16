using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;

namespace Teleboard.Models.Mapping
{
    public class DeviceMap : EntityTypeConfiguration<Device>
    {
        public DeviceMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.TimeZoneId).HasMaxLength(200).IsRequired();
            this.Property(t => t.DeviceId).HasMaxLength(200).IsRequired();
            this.Property(t => t.Name).HasMaxLength(50).IsRequired();
            this.Property(t => t.Description).HasMaxLength(200).IsOptional();
            this.Property(t => t.Version).HasMaxLength(20).IsOptional();

            // Table & Column Mappings
            this.ToTable("Device");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.Devices)
                .HasForeignKey(d => d.TenantId);

            this.HasOptional(t => t.ConnectionType)
                .WithMany(t => t.Devices)
                .HasForeignKey(d => d.ConnectionTypeId);
            
        }
    }
}
