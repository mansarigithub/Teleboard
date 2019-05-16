using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teleboard.Localization.ExtensionMethod;
using System.Threading;
using System.Globalization;

namespace Test
{
    [TestClass]
    public class LocalizationTest
    {
        [TestMethod]
        public void TestLocalization()
        {
            //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fa-IR");
            var localizedValue = "InvalidCode".Localize();
        }
    }
}
