using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Data.Entities;
using WebApi.Helpers;
using WebApi.Services.Extensions;
using WebApi.Data;

namespace WebApi.Services
{
    public partial class ErrorLogService : IErrorLogService
    {
        private readonly DataContext db;

        public ErrorLogService(DataContext context)
        {
            this.db = context;
        }

        public async Task<bool> InsertException(string error, string Method, string Path, Guid? User = null)
        {
            try
            {
                ErrorLog exception = new ErrorLog
                {
                    RowDate = DateTime.UtcNow,
                    Log = error,
                    Method = Method,
                    Path = Path,
                    UserId = User
                };
                await db.ErrorLogs.AddAsync(exception);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> InsertException(Exception e, Guid? User = null, [System.Runtime.CompilerServices.CallerMemberName] string Method = "", [System.Runtime.CompilerServices.CallerFilePath] string Path = "")
        {
            try
            {
                string log = e.Message;

                Exception ex = e.InnerException;
                while (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
                {
                    log = log + " -> " + ex.Message;
                    ex = ex.InnerException;
                }
                await InsertException(log, Method, Path, User);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ICollection<ErrorLog>> GetAll()
        {
            try
            {
                return await db.ErrorLogs.ToListAsync();
            }
            catch (Exception e)
            {
                await InsertException(e); throw;
            }
        }

        public async Task<IQueryable<ErrorLog>> GetAllQ()
        {
            try
            {
                return db.ErrorLogs;
            }
            catch (Exception e)
            {
                await InsertException(e); throw;
            }
        }

        public async Task<ICollection<ErrorLogDto>> Log()
        {
            try
            {
                return await db.ErrorLogs.Where(t => !t.Disabled)
                    .Include(u => u.User)
                    .Select(z => new ErrorLogDto
                    {
                        Date = z.RowDate,
                        Username = z.User == null ? "System Generated" : z.User.FirstName + z.User.LastName,
                        Useremail = z.User == null ? "" : z.User.Email,
                        Log = z.Log,
                        Method = z.Method,
                        Path = z.Path
                    }).OrderByDescending(a => a.Date).ToListAsync();
            }
            catch (Exception e)
            {
                await InsertException(e); throw;
            }
        }
    }
}