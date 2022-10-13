using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WishList.Business.IServices;
using WishList.Business.Models;
using WishList.WebApi.Auth;

namespace WishList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper, IAuthManager authManager) : base(authManager)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userModel)
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

            if (userModel == null)
            {
                return BadRequest("Not enough parameters to update user.");
            }

            var result = await _userService.UpdateUser(userId, userModel);

            return StatusCode((int)result.resultCode, result.Message);
        }

        [HttpPost("Password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateModel passwordUpdateModel)
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

            if (passwordUpdateModel == null)
            {
                return BadRequest("Not enough parameters to update password.");
            }

            var result = await _userService.UpdatePassword(userId, passwordUpdateModel);

            return StatusCode((int)result.resultCode, result.Message);
        }

        [HttpGet("Info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var userId = GetCurrentUserId(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

            var userDto = await _userService.GetById(userId);
            var userModel = _mapper.Map<UserModel>(userDto);

            return Ok(userModel);
        }
    }
}
