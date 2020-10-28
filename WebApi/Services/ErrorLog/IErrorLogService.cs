using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Entities;
using WebApi.Dtos;

namespace WebApi.Services
{
    public partial interface IErrorLogService
    {
        Task<bool> InsertException(string ErrorMessage, string Method, string Path, Guid? User = null);

        Task<bool> InsertException(Exception e, Guid? User = null, [System.Runtime.CompilerServices.CallerMemberName] string Method = "", [System.Runtime.CompilerServices.CallerFilePath] string Path = "");

        Task<ICollection<ErrorLog>> GetAll();

        Task<IQueryable<ErrorLog>> GetAllQ();

        Task<ICollection<ErrorLogDto>> Log();
    }
}