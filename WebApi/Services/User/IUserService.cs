using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data.Entities;
using WebApi.Dtos;

namespace WebApi.Services
{
    public interface IUserService
    {
        Task<AuthStatusDto> AuthenticateAsync(User user, string password);

        Task<AuthStatusDto> GoogleAuthAsync(string idToken);

        Task<AuthStatusDto> FacebookAuthAsync(string idToken);

        Task<AuthStatusDto> RefreshAsync(string idToken);

        Task<AuthStatusDto> RevokeAccess(Guid UserId);

        IEnumerable<User> GetAll();

        Task<User> GetById(Guid id);

        Task<AuthStatusDto> CreateAsync(User user, string password);

        Task<bool> UpdateAsync(EditUserDto user);

        void Delete(int id);

        Task<bool> AddToRoleAsync(User user, IEnumerable<string> roles);

        Task<bool> ConfirmEmailCodeAsync(string ConfirmationCode, string Email = "");

        Task<bool> IsEmailCodeAsync(string ConfirmationCode, string Email = "");

        Task<bool> ResendEmailConfirmationAsync();

        Task<bool> SendForgotPwdEmailAsync(string email);

        Task<bool> ResetPasswordAsync(string Email, string Password);

        Task<bool?> IsLockedAsync(User user);

        Task<bool?> IsLockedAsync();
    }
}