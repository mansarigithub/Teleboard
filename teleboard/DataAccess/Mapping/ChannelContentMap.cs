using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;

namespace Teleboard.Models.Mapping
{
    public class ChannelContentMap : EntityTypeConfiguration<ChannelContent>
    {
        public ChannelContentMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("ChannelContent");

            // Relationships
            this.HasRequired(t => t.Channel)
                .WithMany(t => t.ChannelContents)
                .HasForeignKey(d => d.ChannelId);

            this.HasRequired(t => t.Content)
                .WithMany(t => t.ChannelContents)
                .HasForeignKey(d => d.ContentId);
        }
    }
}
