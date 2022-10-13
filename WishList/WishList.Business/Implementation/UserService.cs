using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WishList.Business.IServices;
using WishList.Business.Models;
using WishList.Business.ModelsDto;
using WishList.Business.Tools;
using WishList.DAL.Core.Entities;

namespace WishList.Business.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, ICacheService cacheService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _cacheService = cacheService;
        }
        public async Task<UserDto> GetByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = await GetUserRole(user);

            return userDto;
        }

        public async Task<UserDto> GetById(string userId)
        {
            var userDto = _cacheService.Get<UserDto>(userId);
            if (userDto != null)
            {
                return userDto;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            userDto = _mapper.Map<UserDto>(user);
            userDto.Role = await GetUserRole(user);
            _cacheService.AddToCache(userDto, userDto.Id);

            return userDto;
        }

        public async Task<UserDto> SignIn(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);
            if (!result.Succeeded)
            {
                return null;
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = await GetUserRole(user);

            return userDto;
        }

        public async Task<IdentityResult> SignUp(string name, string email, string password)
        {
            var userDto = new UserDto
            {
                UserName = name,
                Email = email,
                Role = "User"
            };

            var user = _mapper.Map<User>(userDto);
            user.Currency = Currency.None;
            var resultCreating = await _userManager.CreateAsync(user, password);
            if (!resultCreating.Succeeded)
            {
                throw new Exception(resultCreating.ToString());
            }

            userDto.Id = await _userManager.GetUserIdAsync(user);

            var resultAdding = await _userManager.AddToRoleAsync(user, userDto.Role);
            if (!resultAdding.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new Exception(resultAdding.ToString());
            }
            _cacheService.AddToCache(userDto, userDto.Id);
            return resultAdding;
        }

        public async Task<RequestResult> UpdatePassword(string userId, PasswordUpdateModel passwordUpdateModel)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.Unauthorized,
                    Message = "User not found."
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, passwordUpdateModel.Password, passwordUpdateModel.NewPassword);
            if (!result.Succeeded)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.BadRequest,
                    Message = "Update failed."
                };
            }

            return new RequestResult
            {
                resultCode = HttpStatusCode.OK,
                Message = "Password updated successfully."
            };
        }

        public async Task<RequestResult> UpdateUser(string userId, UserModel userModel)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.Unauthorized,
                    Message = "User not found."
                };
            }

            _mapper.Map(userModel, user);
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new RequestResult
                {
                    resultCode = HttpStatusCode.BadRequest,
                    Message = "Update failed."
                };
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = await GetUserRole(user);
            _cacheService.AddToCache(userDto, userDto.Id);
            return new RequestResult
            {
                resultCode = HttpStatusCode.OK,
                Message = "Settings updated successfully."
            };
        }

        private async Task<string> GetUserRole(User user)
            => (await _userManager.GetRolesAsync(user))?.FirstOrDefault();
    }
}
