using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Data.Entities;

namespace WebApi.Dtos
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public User ToUser()
        {
            return new User
            {
                Id = Id.GetValueOrDefault(),
                FirstName = FirstName,
                LastName = LastName,
                UserName = Username,
                Email = Email,
                PhoneNumber = PhoneNumber
            };
        }

        public static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                Id = user.Id,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName
            };
        }
    }

    public class ForgotPwdDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string ConfirmationCode { get; set; }

        public string NewPassword { get; set; }
    }

    public class AuthDto
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public int TokenExpiration { get; set; }
        public bool EmailConfirmed { get; set; }
    }

    public class EditUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ConfirmCodeDto
    {
        public string Code { get; set; }
    }
}