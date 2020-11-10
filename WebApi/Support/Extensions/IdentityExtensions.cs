using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using WebApi.Authorization;

namespace WebApi.Support.Extensions
{
    public static class IdentityExtensions
    {
        public static string GivenName(this IIdentity id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)id;
            Claim claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == JwtClaimType.FirstName);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string Surname(this IIdentity id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)id;
            Claim claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == JwtClaimType.LastName);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string Id(this IIdentity id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)id;
            Claim claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == JwtClaimType.UserId);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static Guid? Id(this ClaimsPrincipal user)
        {
            try
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)user.Identity;
                Claim claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == JwtClaimType.UserId);
                if (claim == null) return null;
                else return new Guid(claim.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Email(this ClaimsPrincipal user)
        {
            try
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)user.Identity;
                Claim claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == JwtClaimType.Email);
                if (claim != null) return claim.Value;
                else return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ICollection<Permission> Permissions(this IIdentity id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)id;
            Claim claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == PermissionConstants.ClaimType);
            string Permissions = string.Empty;
            if (claim != null) Permissions = claim.Value;

            if (string.IsNullOrWhiteSpace(Permissions)) return System.Array.Empty<Permission>();
            else return PermissionPackers.UnpackPermissionsFromString(Permissions).ToList();
        }

        /// <summary>
        /// Returns true if user has any of the given permissions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="perms">Permissions</param>
        /// <returns></returns>
        public static bool HasAnyPermissions(this IIdentity id, params Permission[] perms)
        {
            try
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)id;
                Claim claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == PermissionConstants.ClaimType);
                if (claim == null) return false;
                Permission[] UserPermissions = claim.Value.UnpackPermissionsFromString().ToArray();
                return UserPermissions.UserHasAnyPermissions(perms);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if user has any of the given permissions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="perms">Permissions</param>
        /// <returns></returns>
        public static bool HasPermissions(this ClaimsPrincipal user, params Permission[] perms)
        {
            try
            {
                return HasAnyPermissions(user.Identity, perms);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if user has all listed permissions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="perms"></param>
        /// <returns></returns>
        public static bool HasAllPermissions(this IIdentity id, params Permission[] perms)
        {
            try
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)id;
                Claim claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == PermissionConstants.ClaimType);
                if (claim == null) return false;
                Permission[] UserPermissions = claim.Value.UnpackPermissionsFromString().ToArray();
                return UserPermissions.UserHasAllPermissions(perms);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if user has all listed permissions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="perms"></param>
        /// <returns></returns>
        public static bool HasAllPermissions(this ClaimsPrincipal user, params Permission[] perms)
        {
            try
            {
                return HasAllPermissions(user.Identity, perms);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if user has a specific permission, regardless of AcessAll or NotSet permissions
        /// </summary>
        /// <param name="user"></param>
        /// <param name="perms"></param>
        /// <returns></returns>
        public static bool HasSpecificPermission(this IIdentity id, Permission perm)
        {
            try
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)id;
                Claim claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == PermissionConstants.ClaimType);
                if (claim == null) return false;
                Permission[] UserPermissions = claim.Value.UnpackPermissionsFromString().ToArray();
                return UserPermissions.UserHasSpecificPermission(perm);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if user has a specific permission, regardless of AcessAll or NotSet permissions
        /// </summary>
        /// <param name="user"></param>
        /// <param name="perms"></param>
        /// <returns></returns>
        public static bool HasSpecificPermission(this ClaimsPrincipal user, Permission perm)
        {
            try
            {
                return HasSpecificPermission(user.Identity, perm);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}