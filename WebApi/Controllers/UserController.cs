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
            try
            {
                AuthStatusDto Auth = await userService.AuthenticateAsync(userDto.ToUser(), userDto.Password);
                return Auth.Status switch
                {
                    AuthStatus.Ok => Ok(new { Status = AuthStatus.Ok, User = Auth.AuthDto }),
                    AuthStatus.NoEmailConfirm => Ok(new { Status = AuthStatus.NoEmailConfirm, User = Auth.AuthDto }),
                    AuthStatus.NoUsernameOrEmail => BadRequest(new { Status = AuthStatus.NoUsernameOrEmail }),
                    AuthStatus.PasswordRequired => BadRequest(new { Status = AuthStatus.PasswordRequired }),
                    AuthStatus.LoginInvalid => BadRequest(new { Status = AuthStatus.LoginInvalid }),
                    AuthStatus.UserLocked => BadRequest(new { Status = AuthStatus.UserLocked }),
                    AuthStatus.Error => BadRequest(new { Status = AuthStatus.Error }),
                    _ => BadRequest(new { Status = AuthStatus.Error }),
                };
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return BadRequest(new { Status = AuthStatus.Error });
            }
        }

        [AllowAnonymous]
        [HttpPost("googleauth")]
        public async Task<IActionResult> GoogleAuthAsync(AuthToken token)
        {
            try
            {
                AuthStatusDto Auth = await userService.GoogleAuthAsync(token.idToken);

                return Auth.Status switch
                {
                    AuthStatus.Ok => Ok(new { Status = AuthStatus.Ok, User = Auth.AuthDto }),
                    AuthStatus.NoEmailConfirm => Ok(new { Status = AuthStatus.NoEmailConfirm, User = Auth.AuthDto }),
                    AuthStatus.NoEmail => BadRequest(new { Status = AuthStatus.NoEmail }),
                    AuthStatus.UserLocked => BadRequest(new { Status = AuthStatus.UserLocked }),
                    AuthStatus.Error => BadRequest(new { Status = AuthStatus.Error }),
                    _ => BadRequest(new { Status = AuthStatus.Error }),
                };
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return BadRequest(new { Status = AuthStatus.Error });
            }
        }

        [AllowAnonymous]
        [HttpPost("facebookauth")]
        public async Task<IActionResult> FacebookAuthAsync(AuthToken token)
        {
            try
            {
                AuthStatusDto Auth = await userService.FacebookAuthAsync(token.idToken);

                return Auth.Status switch
                {
                    AuthStatus.Ok => Ok(new { Status = AuthStatus.Ok, User = Auth.AuthDto }),
                    AuthStatus.NoEmailConfirm => Ok(new { Status = AuthStatus.NoEmailConfirm, User = Auth.AuthDto }),
                    AuthStatus.NoEmail => BadRequest(new { Status = AuthStatus.NoEmail }),
                    AuthStatus.Error => BadRequest(new { Status = AuthStatus.Error }),
                    _ => BadRequest(new { Status = AuthStatus.Error }),
                };
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return BadRequest(new { Status = AuthStatus.Error });
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync(AuthToken token)
        {
            try
            {
                AuthStatusDto result = await userService.RefreshAsync(token.idToken);
                return result.Status switch
                {
                    AuthStatus.Ok => Ok(new { Status = AuthStatus.Ok, User = result.AuthDto }),
                    AuthStatus.NoEmailConfirm => Ok(new { Status = AuthStatus.Ok, User = result.AuthDto }),
                    AuthStatus.TokenNotValid => BadRequest(new { Status = AuthStatus.TokenNotValid }),
                    AuthStatus.LoginInvalid => BadRequest(new { Status = AuthStatus.LoginInvalid }),
                    AuthStatus.UserLocked => BadRequest(new { Status = AuthStatus.UserLocked }),
                    AuthStatus.TokenRevoked => BadRequest(new { Status = AuthStatus.TokenRevoked }),
                    AuthStatus.Error => BadRequest(new { Status = AuthStatus.Error }),
                    _ => BadRequest(new { Status = AuthStatus.Error }),
                };
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return BadRequest(new { Status = AuthStatus.Error });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(UserDto userDto)
        {
            // map dto to entity
            User user = userDto.ToUser();

            try
            {
                // save
                AuthStatusDto result = await userService.CreateAsync(user, userDto.Password);
                return result.Status switch
                {
                    AuthStatus.NoEmailConfirm => Ok(new { Status = AuthStatus.NoEmailConfirm, User = result.AuthDto }),
                    AuthStatus.UsernameNotValid => BadRequest(new { Status = AuthStatus.UsernameNotValid }),
                    AuthStatus.PasswordRequired => BadRequest(new { Status = AuthStatus.PasswordRequired }),
                    AuthStatus.NoEmail => BadRequest(new { Status = AuthStatus.NoEmail }),
                    AuthStatus.UsernameTaken => BadRequest(new { Status = AuthStatus.UsernameTaken }),
                    AuthStatus.EmailTaken => BadRequest(new { Status = AuthStatus.EmailTaken }),
                    AuthStatus.Error => BadRequest(new { Status = AuthStatus.Error }),
                    _ => BadRequest(new { Status = AuthStatus.Error }),
                };
            }
            catch (Exception e)
            {
                // return error message if there was an exception
                await errorLogService.InsertException(e);
                return BadRequest(new { Status = AuthStatus.Error });
            }
        }

        [AllowAnonymous]
        [HttpPost("forgotpwdsendcode")]
        public async Task<ActionResult> ForgotPasswordEmailAsync(ForgotPwdDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = await userService.SendForgotPwdEmailAsync(model.Email);
                    if (result) return Ok();
                }
                return BadRequest("Could not send confirmation code");
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("forgotpwdconfirm")]
        public async Task<ActionResult> ForgotPasswordAsync(ForgotPwdDto model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("No valid e-mail");
                if (string.IsNullOrWhiteSpace(model.ConfirmationCode) || string.IsNullOrWhiteSpace(model.NewPassword)) return BadRequest("Confirmation Code or Password do not exist");
                if (!await userService.IsEmailCodeAsync(model.ConfirmationCode, model.Email)) return BadRequest("Confirmation Code does not match");
                bool result = await userService.ResetPasswordAsync(model.Email, model.NewPassword);
                if (!result) return BadRequest("Could not use specified password or user not found.");
                return Ok();
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
                request.Title = "Welcome!";
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

        [AllowAnonymous]
        [Authorize] //Authorize without role
        [HttpPost("confirmemail")]
        public async Task<ActionResult> ConfirmEmailAsync(string ConfirmationCode)
        {
            try
            {
                if (string.IsNullOrEmpty(ConfirmationCode)) return BadRequest("Please enter the confirmation code");

                bool? IsLocked = await userService.IsLockedAsync();
                if (IsLocked == null || IsLocked == true) return BadRequest("User Locked");

                bool result = await userService.ConfirmEmailCodeAsync(ConfirmationCode);

                if (!result) return BadRequest("Wrong Code");
                return Ok();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [Authorize] //Authorize without role
        [HttpPost("resendconfirm")]
        public async Task<ActionResult> ResendEmailConfirmationAsync()
        {
            try
            {
                bool result = await userService.ResendEmailConfirmationAsync();
                if (!result) return BadRequest("Could not resend e-mail confirmation");
                return Ok();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
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
                    request.Title = "User Id not found";
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
                request.Title = string.Empty;
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