using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Support;

namespace WebApi.Authorization
{
    public class PermissionClaimsConfiguration : UserClaimsPrincipalFactory<User>
    {
        private readonly DataContext context;

        public PermissionClaimsConfiguration(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor, DataContext context) : base(userManager, optionsAccessor)
        {
            this.context = context;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

            return await ClaimManager.UpdateClaims(identity, context, user, PermissionConstants.SubscriptionEnabled);
        }
    }
}