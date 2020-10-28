// Initial Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// This code has since been adapted and modified

using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Authorization
{
    public static class PermissionPackers
    {
        public static string PackPermissionsIntoString(this IEnumerable<Permission> permissions)
        {
            return permissions.Aggregate("", (s, permission) => s + (char)permission);
        }

        public static IEnumerable<Permission> UnpackPermissionsFromString(this string packedPermissions)
        {
            if (packedPermissions == null)
                throw new ArgumentNullException(nameof(packedPermissions));
            foreach (char character in packedPermissions)
            {
                yield return ((Permission)character);
            }
        }

        public static Permission? FindPermissionViaValue(this ushort permissionValue)
        {
            return (Permission?)permissionValue;
        }
    }
}