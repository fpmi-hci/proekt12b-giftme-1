using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WishList.Business.Models;
using WishList.Business.ModelsDto;
using WishList.Business.Tools;

namespace WishList.Business.IServices
{
    public interface IUserService
    {
        public Task<IdentityResult> SignUp(string name, string email, string password);
        public Task<UserDto> SignIn(string email, string password);
        public Task<UserDto> GetByEmail(string email);
        public Task<UserDto> GetById(string userId);
        public Task<RequestResult> UpdateUser(string userId, UserModel userModel);
        public Task<RequestResult> UpdatePassword(string userId, PasswordUpdateModel passwordUpdateModel);
    }
}
