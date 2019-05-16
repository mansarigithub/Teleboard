using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;

namespace Teleboard.Models.Mapping
{
    public class TimeBoxMap : EntityTypeConfiguration<TimeBox>
    {
        public TimeBoxMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            // Table & Column Mappings
            this.ToTable("TimeBox");

            // Relationships
            this.HasRequired(t => t.Channel)
                .WithMany(t => t.TimeBoxes)
                .HasForeignKey(d => d.ChannelId);

            this.HasRequired(t => t.Device)
                .WithMany(t => t.TimeBoxes)
                .HasForeignKey(d => d.DeviceId);
        }
    }
}
