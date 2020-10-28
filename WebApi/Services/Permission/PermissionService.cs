using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Authorization;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Support.Extensions;
using Z.EntityFramework.Plus;

namespace WebApi.Services
{
    public partial class PermissionService : IPermissionService
    {
        private readonly DataContext db;
        private readonly IErrorLogService errorLogService;

        public PermissionService(
            DataContext context,
            IErrorLogService errorLogService
        )
        {
            this.db = context;
            this.errorLogService = errorLogService;
        }

        public async Task<bool> AddToUser(User user, Permission permission)
        {
            try
            {
                if (user == null) throw new Exception("User cannot be null");
                return await AddToUser(user.Id, permission);
            }
            catch (Exception e)
            {
                if (!(await errorLogService.InsertException(e))) throw;
                return false;
            }
        }

        public async Task<bool> AddToUser(Guid UserId, Permission permission)
        {
            try
            {
                User User = await db.Users.Where(u => u.Id == UserId).SingleOrDefaultAsync();
                if (User == null) throw new Exception("User cannot be null");
                ICollection<Permissions> PermissionCollection = new List<Permissions>();
                Permissions Permission = await db.Permissions.Where(p => (Permission)p.Value == permission).SingleOrDefaultAsync();
                if (Permission != null)
                {
                    PermissionCollection.Add(Permission);
                    List<int> ParentId = Permission.ParentIds.ToList();
                    while (ParentId.Any())
                    {
                        Permissions AuxPerm = await db.Permissions.Where(p => p.Id == ParentId.FirstOrDefault()).SingleOrDefaultAsync();
                        if (AuxPerm != null)
                        {
                            PermissionCollection.Add(AuxPerm);
                            ParentId.AddRange(AuxPerm.ParentIds.ToList());
                        }
                    }
                }

                foreach (Permissions perm in PermissionCollection)
                {
                    UserPermissions UserPermissions = new UserPermissions
                    {
                        Permissions = perm,
                        PermissionsId = perm.Id,
                        RowDate = DateTime.UtcNow,
                        User = User,
                        UserId = UserId
                    };

                    await db.UserPermissions.AddAsync(UserPermissions);
                }

                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                if (!(await errorLogService.InsertException(e))) throw;
                return false;
            }
        }

        public async Task<ICollection<SelectListItem>> GetUsersInPermission(Permission permission)
        {
            try
            {
                return await db.Permissions
                    .Include(a => a.Users).ThenInclude(a => a.User)
                    .Where(a => a.Value == (ushort)permission)
                    .SelectMany(a => a.Users)
                    .Where(a => a.User.LockoutEnd == null || a.User.LockoutEnd < DateTime.UtcNow)
                    .Select(a => new SelectListItem
                    {
                        Value = a.User.Id.ToString(),
                        Text = $"{a.User.FirstName} {a.User.LastName}"
                    })
                    .ToListAsync();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e); throw;
            };
        }

        public async Task<bool> RemoveFromUser(Guid UserId, Permission permission)
        {
            try
            {
                User User = await db.Users.Where(u => u.Id == UserId).Include(p => p.Permissions).ThenInclude(p => p.Permissions).SingleOrDefaultAsync();
                List<UserPermissions> Perms = User.Permissions.Where(p => (Permission)p.Permissions.Value == permission).ToList();
                List<int> PermsIds = Perms.Select(a => a.PermissionsId).ToList();
                List<UserPermissions> Children = User.Permissions.Where(a => a.Permissions.ParentIds.Intersect(PermsIds).Any()).ToList();
                db.UserPermissions.RemoveRange(Perms.Union(Children));
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                if (!(await errorLogService.InsertException(e))) throw;
                return false;
            }
        }

        public async Task<bool> HasPermission(Guid UserId, Permission permission)
        {
            try
            {
                return await db.UserPermissions.Include(a => a.Permissions).Where(a => !a.Disabled && a.UserId == UserId && a.Permissions.Value == (ushort)permission).AnyAsync();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                throw;
            }
        }

        public async Task<bool> HasAllPermissions(Guid UserId, params Permission[] permissions)
        {
            try
            {
                List<ushort> Perms = permissions.Select(a => (ushort)a).ToList();
                int Result = await db.UserPermissions.Include(a => a.Permissions).Where(a => !a.Disabled && a.UserId == UserId && Perms.Contains(a.Permissions.Value)).CountAsync();
                return Result == Perms.Count;
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                throw;
            }
        }

        public async Task<bool> HasAnyPermissions(Guid UserId, params Permission[] permissions)
        {
            try
            {
                List<ushort> Perms = permissions.Select(a => (ushort)a).ToList();
                return await db.UserPermissions.Include(a => a.Permissions).Where(a => !a.Disabled && a.UserId == UserId && Perms.Contains(a.Permissions.Value)).AnyAsync();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                throw;
            }
        }

        public async Task<User> GetUserWithPermission(Guid UserId, Permission permission, bool CheckLocked = false)
        {
            try
            {
                IQueryable<User> query = db.Users.Where(a => a.Id == UserId && a.Permissions.Select(a => a.Permissions.Value).Contains((ushort)permission));
                if (CheckLocked) query = query.Where(a => !a.IsLocked());
                return await query.SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e); throw;
            };
        }
    }
}