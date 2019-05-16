using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Teleboard.DomainModel.Core;
using Teleboard.Models.Mapping;

namespace Teleboard.DataAccess.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //Database.SetInitializer<ApplicationDbContext>(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
            this.Configuration.LazyLoadingEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ChannelContentMap());
            modelBuilder.Configurations.Add(new ChannelMap());
            modelBuilder.Configurations.Add(new ContentMap());
            modelBuilder.Configurations.Add(new DeviceMap());
            modelBuilder.Configurations.Add(new TenantMap());
            modelBuilder.Configurations.Add(new TenantUserMap());
            modelBuilder.Configurations.Add(new TimeBoxMap());
            modelBuilder.Configurations.Add(new TranslateMap());
            modelBuilder.Configurations.Add(new LogMap());
            modelBuilder.Configurations.Add(new AuthenticationTokenMap());

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<TenantUser> TenantUsers { get; set; }

        public DbSet<Device> Devices { get; set; } 

        public DbSet<Channel> Channels { get; set; }

        public DbSet<ConnectionType> ConnectionTypes { get; set; }

        public DbSet<ContentType> ContentTypes { get; set; }

        public DbSet<Content> Contents { get; set; }

        public DbSet<ChannelContent> ChannelContents { get; set; }

        public DbSet<TimeBox> TimeBoxes { get; set; }

        public DbSet<Translate> Translates { get; set; }
       
        public DbSet<Log> Logs { get; set; }
        public DbSet<AuthenticationToken> AuthenticationTokens { get; set; }


        public int[] GetTenantsFromUser(string userId, bool isHost)
        {
            if (isHost)
            {
                return this.Tenants.Select(o => o.Id).ToArray();
            }
            else
            {
                return this.TenantUsers.Where(o => o.UserId == userId).Select(o => o.TenantId).ToArray();
            }
        }
    }
}