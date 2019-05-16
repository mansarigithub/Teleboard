using System;
using System.Linq;
using Teleboard.DataAccess.Context;
using Teleboard.DomainModel.Core;

namespace Teleboard.Business.Core
{
    public class AuthenticationTokenBiz : BizBase<AuthenticationToken>
    {
        private ApplicationDbContext Context { get; set; }

        public AuthenticationTokenBiz(ApplicationDbContext context) : base(context)
        {
            Context = context;
        }

        public string GenerateToken(string userId)
        {
            var authToken = Context.AuthenticationTokens.FirstOrDefault(t => t.UserId == userId);
            if (authToken != null) return authToken.Token;

            var token = (Guid.NewGuid().ToString() + Guid.NewGuid().ToString()).Replace("-", "");
            Add(new AuthenticationToken() {
                CreateDate = DateTime.UtcNow,
                Token = token,
                UserId = userId,
            });
            SaveChanges();
            return token;
        }
    }
}
