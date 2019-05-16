using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teleboard.Helper
{
    public static class SettingsHelper
    {

        public static string SmtpHost = "mail.teleboard.ir";

        public static int SmtpPort = 25;

        public static string SmtpEmail = "noreply@teleboard.ir";

        public static string SmtpPassword = "Varzesh123";

        public static int SmtpTimeout = 300000;

        public static bool SmtpUseDefaultCredential = true;

        public static bool SmtpEnableSsl = false;

    }
}