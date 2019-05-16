using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;
using Teleboard.Models;

namespace Teleboard.Models.Mapping
{
    public class ChannelMap : EntityTypeConfiguration<Channel>
    {
        public ChannelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("Channel");
            this.Property(t => t.Description).IsOptional().HasMaxLength(200);
            this.Property(t => t.Name).IsRequired().HasMaxLength(50);

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.Channels)
                .HasForeignKey(d => d.TenantId)
                .WillCascadeOnDelete(false);
        }
    }
}
