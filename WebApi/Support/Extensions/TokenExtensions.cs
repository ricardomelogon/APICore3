using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using WebApi.Authorization;

namespace WebApi.Support.Extensions
{
    public static class TokenExtensions
    {
        public static string GivenName(this JwtSecurityToken Token)
        {
            try
            {
                Claim Claim = Token.Claims.Where(s => s.Type == JwtClaimType.FirstName).FirstOrDefault();
                return (Claim != null) ? Claim.Value : string.Empty;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Surname(this JwtSecurityToken Token)
        {
            try
            {
                Claim Claim = Token.Claims.Where(s => s.Type == JwtClaimType.LastName).FirstOrDefault();
                return (Claim != null) ? Claim.Value : string.Empty;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Guid? Id(this JwtSecurityToken Token)
        {
            try
            {
                Claim Claim = Token.Claims.Where(s => s.Type == JwtClaimType.UserId).FirstOrDefault();
                if(Claim == null) return null;
                return new Guid(Claim.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Email(this JwtSecurityToken Token)
        {
            try
            {
                Claim Claim = Token.Claims.Where(s => s.Type == JwtClaimType.Email).FirstOrDefault();
                return (Claim != null) ? Claim.Value : string.Empty;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ICollection<Permission> Permissions(this JwtSecurityToken Token)
        {
            Claim Claim = Token.Claims.Where(s => s.Type == PermissionConstants.ClaimType).FirstOrDefault();
            string Permissions = string.Empty;
            if (Claim != null) Permissions = Claim.Value;

            if (string.IsNullOrWhiteSpace(Permissions)) return Array.Empty<Permission>();
            else return PermissionPackers.UnpackPermissionsFromString(Permissions).ToList();
        }

        /// <summary>
        /// Returns true if user has any of the given permissions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="perms">Permissions</param>
        /// <returns></returns>
        public static bool HasPermissions(this JwtSecurityToken Token, params Permission[] perms)
        {
            try
            {
                Claim Claim = Token.Claims.Where(s => s.Type == PermissionConstants.ClaimType).FirstOrDefault();
                if (Claim == null) return false;
                Permission[] UserPermissions = Claim.Value.UnpackPermissionsFromString().ToArray();
                return UserPermissions.UserHasAnyPermissions(perms);
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
        public static bool HasAllPermissions(this JwtSecurityToken Token, params Permission[] perms)
        {
            try
            {
                Claim Claim = Token.Claims.Where(s => s.Type == PermissionConstants.ClaimType).FirstOrDefault();
                if (Claim == null) return false;
                Permission[] UserPermissions = Claim.Value.UnpackPermissionsFromString().ToArray();
                return UserPermissions.UserHasAllPermissions(perms);
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
        public static bool HasSpecificPermission(this JwtSecurityToken Token, Permission perm)
        {
            try
            {
                Claim Claim = Token.Claims.Where(s => s.Type == PermissionConstants.ClaimType).FirstOrDefault();
                if (Claim == null) return false;
                Permission[] UserPermissions = Claim.Value.UnpackPermissionsFromString().ToArray();
                return UserPermissions.UserHasSpecificPermission(perm);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}