using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Authorization;
using WebApi.Data.Entities;

namespace WebApi.Services
{
    public partial interface IPermissionService
    {
        Task<bool> AddToUser(User user, Permission permission);

        Task<bool> AddToUser(Guid UserId, Permission permission);

        Task<bool> RemoveFromUser(Guid UserId, Permission permission);

        Task<bool> HasPermission(Guid UserId, Permission permission);

        Task<bool> HasAllPermissions(Guid UserId, params Permission[] permissions);

        Task<bool> HasAnyPermissions(Guid UserId, params Permission[] permissions);

        Task<ICollection<SelectListItem>> GetUsersInPermission(Permission permission);

        Task<User> GetUserWithPermission(Guid UserId, Permission permission, bool CheckLocked = false);
    }
}