using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi.Helpers.Extensions
{
    public static class AuthExtensions
    {
        public static string Id(this ClaimsPrincipal claims)
        {
            Claim id = claims.FindFirst(JwtClaimType.UserId);
            if (id == null) return string.Empty;
            if (string.IsNullOrWhiteSpace(id.Value)) return string.Empty;
            return id.Value;
        }
    }
}
