using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Data.Entities;
using WebApi.Helpers;
using WebApi.Helpers.Extensions;
using WebApi.Services;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> AuthenticateAsync([FromBody]UserDto userDto)
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
        public async Task<IActionResult> GoogleAuthAsync([FromBody]AuthToken token)
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
        public async Task<IActionResult> FacebookAuthAsync([FromBody]AuthToken token)
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
        public async Task<IActionResult> RefreshAsync([FromBody]AuthToken token)
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
        public async Task<IActionResult> RegisterAsync([FromBody]UserDto userDto)
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
        public async Task<ActionResult> ForgotPasswordEmailAsync([FromBody]ForgotPwdDto model)
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
        public async Task<ActionResult> ForgotPasswordAsync([FromBody]ForgotPwdDto model)
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

        [Authorize]
        [HttpPost("revoke")]
        public async Task<ActionResult> RevokeAccess()
        {
            try
            {
                string UserId = User.Id();
                AuthStatusDto result = await userService.RevokeAccess(UserId);
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
        public async Task<ActionResult> ConfirmEmailAsync([FromBody]string ConfirmationCode)
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
        public async Task<IActionResult> GetById(string id)
        {
            User user = await userService.GetById(id);
            UserDto userDto = UserDto.ToDto(user);
            return Ok(userDto);
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody]EditUserDto user)
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
    }
}
