using System.Web;
using Teleboard.Common.Enum;
using Teleboard.DomainModel.Core;

namespace Teleboard.UI.Infrastructure.Mvc
{
    public static class SessionManager
    {
        public static void StoreUserIdentity(ApplicationUser user)
        {
            HttpContext.Current.Session[SessionKeys.UserIdentity.ToString()] = user;
        }

        public static ApplicationUser GetUserIdentity()
        {
            return HttpContext.Current.Session[SessionKeys.UserIdentity.ToString()] as ApplicationUser;
        }

        public static void Abandon()
        {
            HttpContext.Current.Session.Abandon();
        }
    }
}