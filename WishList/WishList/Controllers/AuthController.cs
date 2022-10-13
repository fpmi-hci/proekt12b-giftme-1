using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WishList.Business.IServices;
using WishList.Business.Models;
using WishList.WebApi.Auth;

namespace WishList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IAuthManager _authManager;

        public AuthController(IUserService userService, IEmailService emailService, IAuthManager authManager)
        {
            _userService = userService;
            _emailService = emailService;
            _authManager = authManager;
        }

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<ActionResult> SignIn([FromBody] SignInModel signInModel)
        {
            var userDto = await _userService.SignIn(signInModel.Email, signInModel.Password);
            if (userDto == null)
            {
                return Unauthorized("Wrong password. Try again");
            }

            var jwtToken = _authManager.GenerateToken(userDto, DateTime.Now);

            return Ok(jwtToken);
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<ActionResult> SignUp([FromBody] SignUpModel signUpModel)
        {
            var userDto = await _userService.GetByEmail(signUpModel.Email);
            if (userDto != null)
            {
                return BadRequest("Account with such email already exists.");
            }

            var userAdded = await _userService.SignUp(signUpModel.UserName, signUpModel.Email, signUpModel.Password);
            if (!userAdded.Succeeded)
            {
                return BadRequest("User can not be signed up. Try registering once again.");
            }

            userDto = await _userService.GetByEmail(signUpModel.Email);
            var code = await _emailService.GenerateEmailConfirmationToken(userDto);
            var callback = Url.Action(nameof(ConfirmEmail), "Auth", new { email = userDto.Email, code = code },
                Request.Scheme);
            await _emailService.SendEmailConfirmMessage(userDto.Email, callback);

            return Ok("Signed up successfully.");
        }

        [AllowAnonymous]
        [HttpGet("confirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<ActionResult> ConfirmEmail(string email, string code)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Wrong email or token.");
            }

            var emailConfirmed = await _emailService.ConfirmEmail(email, code);
            if (emailConfirmed.Succeeded)
            {
                return Ok();
            }

            return BadRequest("The email wasn't confirmed.");
        }
    }
}
