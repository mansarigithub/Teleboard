using System;
using Teleboard.Common.Enum;

namespace Teleboard.DomainModel.Core
{
    public class AuthenticationToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}