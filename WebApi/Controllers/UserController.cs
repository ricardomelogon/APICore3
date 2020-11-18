using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Authorization;
using WebApi.Data.Entities;
using WebApi.Dtos;
using WebApi.Services;
using WebApi.Support;
using WebApi.Support.Extensions;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly IErrorLogService errorLogService;

        public UserController(
            IUserService userService,
            IErrorLogService errorLogService)
        {
            this.userService = userService;
            this.errorLogService = errorLogService;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> AuthenticateAsync(UserDto userDto)
        {
            Guid? UserId = null;
            RequestFeedback<AuthDto> request = new RequestFeedback<AuthDto>();
            try
            {
                UserId = User.Id();

                AuthStatusDto Auth = await userService.AuthenticateAsync(userDto.ToUser(), userDto.Password);
                request.Status = Auth.Status;
                request.Data = Auth.AuthDto;
                request.Success = Auth.Status switch
                {
                    AuthStatus.Ok => true,
                    AuthStatus.NoEmailConfirm => true,
                    AuthStatus.NoUsernameOrEmail => false,
                    AuthStatus.PasswordRequired => false,
                    AuthStatus.LoginInvalid => false,
                    AuthStatus.UserLocked => false,
                    AuthStatus.Error => false,
                    _ => false,
                };
                if (request.Success) return Ok(request);
                else return BadRequest(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("googleauth")]
        public async Task<IActionResult> GoogleAuthAsync(AuthToken token)
        {
            Guid? UserId = null;
            RequestFeedback<AuthDto> request = new RequestFeedback<AuthDto>();
            try
            {
                UserId = User.Id();

                AuthStatusDto Auth = await userService.GoogleAuthAsync(token.idToken);
                request.Status = Auth.Status;
                request.Data = Auth.AuthDto;
                request.Success = Auth.Status switch
                {
                    AuthStatus.Ok => true,
                    AuthStatus.NoEmailConfirm => true,
                    AuthStatus.NoEmail => false,
                    AuthStatus.UserLocked => false,
                    AuthStatus.Error => false,
                    _ => false,
                };
                if (request.Success) return Ok(request);
                else return BadRequest(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("facebookauth")]
        public async Task<IActionResult> FacebookAuthAsync(AuthToken token)
        {
            Guid? UserId = null;
            RequestFeedback<AuthDto> request = new RequestFeedback<AuthDto>();
            try
            {
                UserId = User.Id();

                AuthStatusDto Auth = await userService.FacebookAuthAsync(token.idToken);
                request.Status = Auth.Status;
                request.Data = Auth.AuthDto;
                request.Success = Auth.Status switch
                {
                    AuthStatus.Ok => true,
                    AuthStatus.NoEmailConfirm => true,
                    AuthStatus.NoEmail => false,
                    AuthStatus.Error => false,
                    _ => false,
                };
                if (request.Success) return Ok(request);
                else return BadRequest(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync(AuthToken token)
        {
            Guid? UserId = null;
            RequestFeedback<AuthDto> request = new RequestFeedback<AuthDto>();
            try
            {
                UserId = User.Id();

                AuthStatusDto result = await userService.RefreshAsync(token.idToken);
                request.Status = result.Status;
                request.Data = result.AuthDto;
                request.Success = result.Status switch
                {
                    AuthStatus.Ok => true,
                    AuthStatus.NoEmailConfirm => true,
                    AuthStatus.TokenNotValid => false,
                    AuthStatus.LoginInvalid => false,
                    AuthStatus.UserLocked => false,
                    AuthStatus.TokenRevoked => false,
                    AuthStatus.Error => false,
                    _ => false,
                };
                return Ok(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(UserDto userDto)
        {
            Guid? UserId = null;
            RequestFeedback<AuthDto> request = new RequestFeedback<AuthDto>();
            try
            {
                UserId = User.Id();
                User user = userDto.ToUser();
                AuthStatusDto result = await userService.CreateAsync(user, userDto.Password);
                request.Status = result.Status;
                request.Data = result.AuthDto;
                request.Success = result.Status switch
                {
                    AuthStatus.NoEmailConfirm => true,
                    AuthStatus.UsernameNotValid => true,
                    AuthStatus.PasswordRequired => false,
                    AuthStatus.NoEmail => false,
                    AuthStatus.UsernameTaken => false,
                    AuthStatus.EmailTaken => false,
                    AuthStatus.Error => false,
                    _ => false,
                };
                if (request.Success) return Ok(request);
                else return BadRequest(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("forgotpwdsendcode")]
        public async Task<ActionResult> ForgotPasswordEmailAsync(ForgotPwdDto model)
        {
            RequestFeedback request = new RequestFeedback();
            try
            {
                if (!TextHelper.IsValidEmail(model.Email)) throw new Exception("E-mail not valid");
                request.Success = await userService.SendForgotPwdEmailAsync(model.Email);
                if (request.Success) request.Message = "New e-mail code has been sent successfully";
                return Ok(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return BadRequest(request);
            }
        }

        [AllowAnonymous]
        [HttpPost("forgotpwdconfirm")]
        public async Task<ActionResult> ForgotPasswordAsync(ForgotPwdDto model)
        {
            RequestFeedback request = new RequestFeedback();
            try
            {
                if (string.IsNullOrWhiteSpace(model.ConfirmationCode) || string.IsNullOrWhiteSpace(model.NewPassword) || !TextHelper.IsValidEmail(model.Email))
                {
                    request.Message = "Please enter a valid confirmation code, password and email";
                    return Ok(request);
                }

                if (!await userService.IsEmailCodeAsync(model.ConfirmationCode, model.Email))
                {
                    request.Message = "The given confirmation code is not valid";
                    return Ok(request);
                }

                request.Success = await userService.ResetPasswordAsync(model.Email, model.NewPassword);
                if (!request.Success) return Ok(request);
                request.Message = "Your password has been changed successfully";
                return Ok(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return BadRequest();
            }

        }

        [AllowAnonymous]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            Guid? UserId = null;
            RequestFeedback request = new RequestFeedback();
            try
            {
                UserId = User.Id();
                request.Success = true;
                request.Message = "Welcome!";
                return Ok(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<ActionResult> RevokeAccess()
        {
            try
            {
                Guid? UserId = User.Id();
                if (!UserId.HasValue) throw new Exception("User Id not found");
                AuthStatusDto result = await userService.RevokeAccess(UserId.Value);
                return result.Status switch
                {
                    AuthStatus.Ok => Ok(new { Status = AuthStatus.Ok, User = result.AuthDto }),
                    AuthStatus.LoginInvalid => BadRequest(new { Status = AuthStatus.LoginInvalid }),
                    AuthStatus.NoEmailConfirm => BadRequest(new { Status = AuthStatus.PasswordRequired }),
                    AuthStatus.Error => BadRequest(new { Status = AuthStatus.Error }),
                    _ => BadRequest(new { Status = AuthStatus.Error }),
                };
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmCodeDto model)
        {
            Guid? UserId = null;
            RequestFeedback request = new RequestFeedback();
            try
            {
                UserId = User.Id();
                if (string.IsNullOrEmpty(model.Code)) return BadRequest("Please enter the confirmation code");

                bool? IsLocked = await userService.IsLockedAsync();
                if (IsLocked == null || IsLocked == true) return BadRequest("User Locked");

                request.Success = await userService.ConfirmEmailCodeAsync(model.Code);
                if (request.Success)
                {
                    request.Message = "Confirmation code successfully verified";
                    request.Status = AuthStatus.Ok;
                    return Ok(request);
                }
                else return BadRequest(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [Authorize] //Authorize without role
        [HttpPost("resendconfirm")]
        public async Task<ActionResult> ResendEmailConfirmationAsync()
        {
            Guid? UserId = null;
            RequestFeedback request = new RequestFeedback();
            try
            {
                UserId = User.Id();

                request.Success = await userService.ResendEmailConfirmationAsync();
                if (request.Success)
                {
                    request.Message = "New e-mail code has been sent successfully";
                    return Ok(request);
                }
                return BadRequest("Could not resend e-mail confirmation");
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            IEnumerable<User> users = userService.GetAll();
            IList<UserDto> userDtos = users.Select(a => UserDto.ToDto(a)).ToList();
            return Ok(userDtos);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetById(Guid? id)
        {
            Guid? UserId = null;
            RequestFeedback<UserDto> request = new RequestFeedback<UserDto>();
            try
            {
                UserId = User.Id();
                if (!UserId.HasValue || !id.HasValue)
                {
                    request.Message = "User Id not found";
                    throw new Exception("User Id not found");
                }

                User user = await userService.GetById(id.Value);
                request.Data = UserDto.ToDto(user);
                return Ok(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }

        [HttpPut("update")]
        public IActionResult Update(EditUserDto user)
        {
            try
            {
                // save
                userService.UpdateAsync(user);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            userService.Delete(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public string Test()
        {
            return "v0.0.3";
        }

        [HttpGet("testauth")]
        public string TestAuth()
        {
            return "Worked!";
        }

        [HttpGet("testperm")]
        [Permitted(Permission.AccessAll)]
        public string TestPerm()
        {
            return "Permitted!";
        }

        [HttpGet("double")]
        [Permitted(Permission.AccessAll)]
        public async Task<IActionResult> Double(int Value)
        {
            Guid? UserId = null;
            RequestFeedback<int> request = new RequestFeedback<int>();
            try
            {
                UserId = User.Id();
                request.Data = Value * 2;
                request.Success = true;
                request.Message = string.Empty;
                return Ok(request);
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e, UserId);
                return BadRequest();
            }
        }
    }
}