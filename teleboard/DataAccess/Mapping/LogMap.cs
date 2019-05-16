using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;
using Teleboard.Models;

namespace Teleboard.Models.Mapping
{
    public class LogMap : EntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Type)
                .IsRequired();

            this.Property(t => t.Description)
                .IsOptional()
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("Log");

            // Relationships
        }
    }
}
