using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teleboard.Localization.ExtensionMethod;
using Teleboard;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Teleboard.DataAccess.Context;
using Teleboard.DomainModel.Core;
using System.Linq;
using System.Data.Entity;

namespace Test
{
    [TestClass]
    public class SecurityTest
    {
        [TestMethod]
        public void TestSqlInjection1()
        {
            var context = new ApplicationDbContext();
            var inputString = "ali --";
            var users = context.Users.Where(u => u.FirstName == inputString).ToList();
        }

        [TestMethod]
        public void TestSqlInjection2()
        {
            var context = new ApplicationDbContext();
            var inputString = "ali OR 1=1";
            var users = context.Users.Where(u => u.FirstName == inputString).ToList();
        }


        [TestMethod]
        public void changeData()
        {
            var context = new ApplicationDbContext();
            var device = context.Devices.Include(d => d.TimeBoxes).Single(d => d.Id == 17);
            var clientLocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(device.TimeZoneId));
            var clientDayName = clientLocalDateTime.DayOfWeek.ToString().ToUpper();

            var t = device.TimeBoxes.Where(tb =>
                tb.WeekDay.ToUpper() == clientDayName &&
                new TimeSpan(tb.FromHour, tb.FromMinute, 0) < clientLocalDateTime.TimeOfDay &&
                clientLocalDateTime.TimeOfDay < new TimeSpan(tb.ToHour, tb.ToMinute, 0))
                .ToList();
            context.SaveChanges();
        }

    }
}
