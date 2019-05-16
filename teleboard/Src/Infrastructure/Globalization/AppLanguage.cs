using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Teleboard.Common.Enum;
using Teleboard.DomainModel.Core;
using Teleboard.Localization;

namespace Teleboard.UI.Infrastructure.Globalization
{
    public static class AppLanguage
    {
        public static IEnumerable<SelectListItem> GetSupportedLanguagesListItems()
        {
            return new List<SelectListItem>() {
                        new SelectListItem() { Text = Resources.English, Value = Language.English.ToString() },
                        new SelectListItem() { Text = Resources.Farsi, Value = Language.Farsi.ToString() },
                    }; ;
        }

        public static void SetLanguage(CultureInfo cultureInfo)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}