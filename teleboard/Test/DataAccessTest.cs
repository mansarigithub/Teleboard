using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teleboard.Localization.ExtensionMethod;
using Teleboard;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Teleboard.DataAccess.Context;
using Teleboard.DomainModel.Core;
using Teleboard.Common.Enum;
using System.Linq;
using System.Data.Entity;
using Teleboard.Common.ExtensionMethod;

namespace Test
{
    [TestClass]
    public class DataAccessTest
    {

        [TestMethod]
        public void InitilizeDB_Level1()
        {
            var context = new ApplicationDbContext();
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            #region Roles
            roleManager.Create(new ApplicationRole()
            {
                Id = "Host",
                Name = "Host",
                Description = "Host",
            });
            roleManager.Create(new ApplicationRole()
            {
                Id = "TenantAdmin",
                Name = "TenantAdmin",
                Description = "TenantAdmin",
            });
            roleManager.Create(new ApplicationRole()
            {
                Id = "ContentCreator",
                Name = "ContentCreator",
                Description = "ContentCreator",
            });
            roleManager.Create(new ApplicationRole()
            {
                Id = "Advertiser",
                Name = "Advertiser",
                Description = "Advertiser",
            });

            #endregion

            #region Users
            userManager.Create(new ApplicationUser()
            {
                Email = "host@teleboard.ca",
                UserName = "host@teleboard.ca",
                EmailConfirmed = true,
            }, "12345678");
            userManager.AddToRole(userManager.FindByName("host@teleboard.ca").Id, "Host");
            #endregion

            #region Content Types
            context.ContentTypes.AddRange(new ContentType[]
            {
                new ContentType()
                {
                    Name = "image/jpeg",
                },
                new ContentType()
                {
                    Name = "image/png",
                },
                new ContentType()
                {
                    Name = "image/gif",
                },
                new ContentType()
                {
                    Name = "video/mp4",
                },

                new ContentType()
                {
                    Name = "image/bmp",
                },

            });
            #endregion

            context.SaveChanges();
        }

        [TestMethod]
        public void InitilizeDB_Level2()
        {
            var context = new ApplicationDbContext();
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            #region Tenant
            var tenant1 = new Tenant()
            {
                Name = "Tenant 1",
                Description = "Tenant 1 Desc",
                SubscriptionKey = Guid.NewGuid(),
                ChennalModeration = true,
                TimeBoxModeration = true,
                ContentModeration = true
            };
            context.Tenants.Add(tenant1);
            #endregion

            #region User (Tenant Admins)
            userManager.Create(new ApplicationUser()
            {
                Email = "admin@tenant1.com",
                UserName = "admin@tenant1.com",
                EmailConfirmed = true,
            }, "12345678");
            var tenant1Admin = userManager.FindByName("admin@tenant1.com");
            tenant1Admin.TenantUsers.Add(new TenantUser()
            {
                Tenant = tenant1,
                User = tenant1Admin,
            });
            userManager.AddToRole(tenant1Admin.Id, AppRole.TenantAdmin.ToString());
            #endregion

            #region Device
            var device1 = new Device()
            {
                Name = "Device 1",
                Description = "Device 1 Desc",
                DeviceId = "1",
                RegisteredOnUtc = DateTime.UtcNow,
                Tenant = tenant1,
            };
            context.Devices.Add(device1);
            #endregion

            #region Channel
            var channel1 = new Channel()
            {
                Name = "Channel 1",
                Description = "Channel 1 Desc",
                Tenant = tenant1,
                Flag = true,
            };
            context.Channels.Add(channel1);
            #endregion



            context.SaveChanges();
        }

        [TestMethod]
        public void t()
        {
            var context = new ApplicationDbContext();
            context.Contents.ToList()
                .ForEach(c => c.Source = c.Source.Substring(1));
            context.SaveChanges();
        }
    }
}
