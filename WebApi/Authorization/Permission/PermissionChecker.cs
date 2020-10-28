// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;

namespace WebApi.Authorization
{
    public static class PermissionChecker
    {
        /// <summary>
        /// This is used by the policy provider to check the permission name string
        /// </summary>
        /// <param name="packedPermissions"></param>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        public static bool ThisPermissionIsAllowed(this string packedPermissions, string permissionName)
        {
            Permission[] usersPermissions = packedPermissions.UnpackPermissionsFromString().ToArray();
            Permission[] allowedPermissions = permissionName.UnpackPermissionsFromString().ToArray();

            return usersPermissions.UserHasAnyPermissions(allowedPermissions);
        }

        /// <summary>
        /// This is the main checker of whether a user permissions allows them to access something with the given permission
        /// </summary>
        /// <param name="usersPermissions"></param>
        /// <param name="permissionToCheck"></param>
        /// <returns></returns>
        public static bool UserHasThisPermission(this Permission[] usersPermissions, Permission permissionToCheck)
        {
            return usersPermissions.Contains(permissionToCheck) || usersPermissions.Contains(Permission.AccessAll);
        }

        /// <summary>
        /// This is the main checker of whether a user permissions allows them to access something with the given permission
        /// </summary>
        /// <param name="usersPermissions"></param>
        /// <param name="permissionToCheck"></param>
        /// <returns></returns>
        public static bool UserHasAnyPermissions(this Permission[] usersPermissions, Permission[] permissionsToCheck)
        {
            return usersPermissions.Contains(Permission.AccessAll) || usersPermissions.Intersect(permissionsToCheck).Any();
        }

        /// <summary>
        /// This is the main checker of whether a user permissions allows them to access something with the given permission
        /// </summary>
        /// <param name="usersPermissions"></param>
        /// <param name="permissionToCheck"></param>
        /// <returns></returns>
        public static bool UserHasAllPermissions(this Permission[] usersPermissions, Permission[] permissionsToCheck)
        {
            return usersPermissions.Contains(Permission.AccessAll) || permissionsToCheck.All(a => usersPermissions.Contains(a));
        }

        public static bool UserHasSpecificPermission(this Permission[] usersPermissions, Permission perm)
        {
            return usersPermissions.Contains(perm);
        }
    }
}