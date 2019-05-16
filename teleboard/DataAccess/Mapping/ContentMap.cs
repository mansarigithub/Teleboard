using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;

namespace Teleboard.Models.Mapping
{
    public class ContentMap : EntityTypeConfiguration<Content>
    {
        public ContentMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("Content");
            this.Property(t => t.Description).HasMaxLength(200).IsRequired();

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.Contents)
                .HasForeignKey(d => d.TenantId);

            this.HasRequired(t => t.ContentType)
                .WithMany(t => t.Contents)
                .HasForeignKey(d => d.ContentTypeId);

            this.HasOptional(t => t.Creator)
                .WithMany(t => t.Contents)
                .HasForeignKey(d => d.CreatorId)
                .WillCascadeOnDelete(false);
        }
    }
}
