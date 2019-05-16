using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Teleboard.DomainModel.Core;
using Teleboard.Models;

namespace Teleboard.Models.Mapping
{
    public class AuthenticationTokenMap : EntityTypeConfiguration<AuthenticationToken>
    {
        public AuthenticationTokenMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(64);

            // Table & Column Mappings
            this.ToTable("AuthenticationToken");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.AuthenticationTokens)
                .HasForeignKey(d => d.UserId);
        }
    }
}
