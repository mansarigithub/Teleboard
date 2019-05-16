using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Teleboard.DataAccess.Context;
using Teleboard.DomainModel.Core;
using Teleboard.Models;

namespace Teleboard
{
    public class Translator
    {
        private static List<Translate> translates = null;

        public static void Reset()
        {
            translates = null;
        }

        public static CultureInfo UserCulture
        {
            get
            {
                var defaultCulture = ConfigurationManager.AppSettings["defaultCulture"];
                if (string.IsNullOrWhiteSpace(defaultCulture))
                {
                    var userLanguages = HttpContext.Current.Request.UserLanguages.FirstOrDefault(o => !o.StartsWith("en"));
                    CultureInfo ci = new CultureInfo((userLanguages.Count() > 0) ? userLanguages.Split(';')[0] : "en-US");
                    return ci;
                }
                else
                {
                    return new CultureInfo(defaultCulture);
                }
            }
        }

        public static bool IsRightToLeft
        {
            get
            {
                try
                {
                    return UserCulture.TextInfo.IsRightToLeft;
                }
                catch
                {
                    return false;
                }
            }
        } 

        public static string Get(string name)
        {
            try
            {
                if (translates == null)
                {
                    try
                    {
                        using (var db = new ApplicationDbContext())
                        {
                            translates = db.Translates.ToList();
                        }
                    }
                    catch { }
                }

                var translate = (translates ?? new List<Translate>())
                    .FirstOrDefault(o => o.Culture == UserCulture.TextInfo.CultureName && o.Name.ToLower() == name.ToLower());
                return (translate == null) ? name : translate.Value;
            }
            catch
            {
                return name;
            }
        }
    }
}