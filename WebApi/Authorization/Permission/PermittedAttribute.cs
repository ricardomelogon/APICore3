using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization
{
    internal class PermittedAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// If the user has any of the given permissions they will be allowed to proceed.
        /// To Enforce multiple permissions use mutiple permitted attributes.
        /// To Allow for any of multiple permissions, add all into a single permitted attribute.
        /// </summary>
        /// <param name="permission"></param>
        public PermittedAttribute(params Permission[] permission) : base(permission.PackPermissionsIntoString())
        {
        }
    }
}