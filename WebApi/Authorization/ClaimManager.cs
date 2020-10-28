using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Support.Extensions;

#pragma warning disable CS0162 // Unreachable code detected is expected as parts of the code are flag bound

namespace WebApi.Authorization
{
    public static class ClaimManager
    {
        public static async Task<ClaimsIdentity> UpdateClaims(ClaimsIdentity identity, DataContext db, User user, bool RefreshSubscription = false)
        {
            PermissionCalculator PermissionCalculator = new PermissionCalculator(db);
            string Permissions = await PermissionCalculator.CalculatePermissions(user.Id);
            ICollection<Claim> PermissionClaims = identity.FindAll(PermissionConstants.ClaimType).ToList();
            foreach (Claim claim in PermissionClaims) identity.TryRemoveClaim(claim);
            identity.AddClaim(new Claim(PermissionConstants.ClaimType, Permissions));

            string FirstName = "User";
            if (!string.IsNullOrEmpty(user.FirstName)) FirstName = user.FirstName;
            identity.SetClaim(ClaimTypes.GivenName, FirstName);

            string LastName = string.Empty;
            if (!string.IsNullOrEmpty(user.LastName)) LastName = user.LastName;
            identity.SetClaim(ClaimTypes.Surname, LastName);

            string Email = string.Empty;
            if (!string.IsNullOrEmpty(user.Email)) Email = user.Email;
            identity.SetClaim(ClaimTypes.Email, Email);

            if (PermissionConstants.RefreshEnabled)
            {
                IEnumerable<Claim> RefreshClaims = identity.FindAll(PermissionConstants.RefreshClaimType).ToList();
                foreach (Claim claim in RefreshClaims.ToList()) identity.TryRemoveClaim(claim);
                identity.AddClaim(new Claim(PermissionConstants.RefreshClaimType, DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture)));
            }

            if (RefreshSubscription)
            {
                IEnumerable<Claim> SubscriptionClaims = identity.FindAll(PermissionConstants.SubscriptionClaimType).ToList();
                foreach (Claim claim in SubscriptionClaims.ToList()) identity.TryRemoveClaim(claim);
                DateTime SubscriptionDate = await PermissionCalculator.CalculateSubscription();

                if (DateTime.UtcNow < SubscriptionDate)
                {
                    identity.AddClaim(new Claim(PermissionConstants.SubscriptionClaimType, DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture)));
                }
            }

            return identity;
        }
    }
}