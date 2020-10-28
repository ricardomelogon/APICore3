// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Authorization;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Services;

#pragma warning disable CS0162 // Unreachable code detected is expected as parts of the code are flag bound

namespace WebApi.Authorization
{
    //thanks to https://www.jerriepelser.com/blog/creating-dynamic-authorization-policies-aspnet-core/

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IErrorLogService errorLogService;
        private readonly IUserService userService;

        public PermissionHandler(IErrorLogService errorLogService, IUserService userService)
        {
            this.errorLogService = errorLogService;
            this.userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            try
            {
                Claim permissionsClaim = context.User.Claims.SingleOrDefault(c => c.Type == PermissionConstants.ClaimType);
                // If user does not have the scope claim, end challenge
                if (permissionsClaim == null) return;

                if (PermissionConstants.RefreshEnabled)
                {
                    Claim permissionsRefreshClaim = context.User.Claims.SingleOrDefault(c => c.Type == PermissionConstants.RefreshClaimType);
                    DateTime RefreshDate = DateTime.MinValue;
                    if (permissionsRefreshClaim != null) RefreshDate = DateTime.Parse(permissionsRefreshClaim.Value, CultureInfo.InvariantCulture).AddSeconds(PermissionConstants.RefreshTime);
                    if (RefreshDate < DateTime.UtcNow)
                    {
                        bool RefreshSubscription = false;
                        if (PermissionConstants.SubscriptionEnabled)
                        {
                            Claim subscriptionRefreshClaim = context.User.Claims.SingleOrDefault(c => c.Type == PermissionConstants.SubscriptionClaimType);
                            DateTime SubscriptionDate = DateTime.MinValue;
                            if (subscriptionRefreshClaim != null) SubscriptionDate = DateTime.Parse(subscriptionRefreshClaim.Value, CultureInfo.InvariantCulture).AddSeconds(PermissionConstants.SubscriptionRefreshTime);
                            if (SubscriptionDate < DateTime.UtcNow) RefreshSubscription = true;
                        }
                        await userService.UpdateClaims(context.User.Identity, RefreshSubscription);
                        permissionsClaim = context.User.Claims.SingleOrDefault(c => c.Type == PermissionConstants.ClaimType);
                        if (RefreshSubscription)
                        {
                            Claim subscriptionRefreshClaim = context.User.Claims.SingleOrDefault(c => c.Type == PermissionConstants.SubscriptionClaimType);
                            if (subscriptionRefreshClaim == null) return;
                            DateTime SubscriptionDate = DateTime.Parse(subscriptionRefreshClaim.Value, CultureInfo.InvariantCulture).AddSeconds(PermissionConstants.SubscriptionRefreshTime);
                            if (SubscriptionDate < DateTime.UtcNow) return;
                        }
                        if (permissionsClaim == null) return;
                    }
                }

                if (permissionsClaim.Value.ThisPermissionIsAllowed(requirement.PermissionName)) context.Succeed(requirement);

                return;
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return;
            }
        }
    }
}