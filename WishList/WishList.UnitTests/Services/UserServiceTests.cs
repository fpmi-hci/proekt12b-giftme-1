using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using WishList.Business.Implementation;
using WishList.Business.IServices;
using WishList.Business.Models;
using WishList.Business.ModelsDto;
using WishList.DAL.Core.Entities;
using Xunit;

namespace WishList.UnitTests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async void SignUp_ReturnsSuccess()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeMapper = A.Fake<IMapper>();
            var fakeCacheService = A.Fake<ICacheService>();

            A.CallTo(() => fakeMapper.Map<User>(A<UserDto>._))
                .Returns(new User());
            A.CallTo(() => fakeUserManager.CreateAsync(A<User>._, A<string>._))
                .Returns(IdentityResult.Success);
            A.CallTo(() => fakeUserManager.GetUserIdAsync(A<User>._))
                .Returns(string.Empty);
            A.CallTo(() => fakeUserManager.AddToRoleAsync(A<User>._, A<string>._))
                .Returns(IdentityResult.Success);
            var entry = A.Fake<ICacheEntry>();
            A.CallTo(() => fakeCacheService.AddToCache(A<UserDto>._, A<string>._))
                .DoesNothing();

            var userService = new UserService(fakeUserManager, null, fakeMapper, fakeCacheService);
            var result = await userService.SignUp("name", "email", "password");

            Assert.Equal(IdentityResult.Success, result);
        }

        [Fact]
        public void SignUp_ThrowsExceptionCreating()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeMapper = A.Fake<IMapper>();

            A.CallTo(() => fakeMapper.Map<User>(A<UserDto>._))
                .Returns(new User());
            A.CallTo(() => fakeUserManager.CreateAsync(A<User>._, A<string>._))
                .Returns(IdentityResult.Failed());

            var userService = new UserService(fakeUserManager, null, fakeMapper, null);
            var exception = Assert.ThrowsAsync<Exception>(() => userService.SignUp("name", "email", "password"));
        }

        [Fact]
        public void SignUp_ThrowsExceptionAdding()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeMapper = A.Fake<IMapper>();

            A.CallTo(() => fakeMapper.Map<User>(A<UserDto>._))
                .Returns(new User());
            A.CallTo(() => fakeUserManager.CreateAsync(A<User>._, A<string>._))
                .Returns(IdentityResult.Success);
            A.CallTo(() => fakeUserManager.GetUserIdAsync(A<User>._))
                .Returns(string.Empty);
            A.CallTo(() => fakeUserManager.AddToRoleAsync(A<User>._, A<string>._))
                .Returns(IdentityResult.Failed());

            var userService = new UserService(fakeUserManager, null, fakeMapper, null);
            Assert.ThrowsAsync<Exception>(() => userService.SignUp("name", "email", "password"));
        }

        [Fact]
        public async void SignIn_ReturnsUserDto()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeSignInManager = A.Fake<SignInManager<User>>();
            var fakeMapper = A.Fake<IMapper>();

            A.CallTo(() => fakeUserManager.FindByEmailAsync(A<string>._))
                .Returns(new User());
            A.CallTo(() => fakeSignInManager.PasswordSignInAsync(A<string>._, A<string>._, A<bool>._, A<bool>._))
                .Returns(SignInResult.Success);
            A.CallTo(() => fakeMapper.Map<UserDto>(A<User>._))
                .Returns(new UserDto());

            var userService = new UserService(fakeUserManager, fakeSignInManager, fakeMapper, null);
            var result = await userService.SignIn("email", "password");

            Assert.IsAssignableFrom<UserDto>(result);
        }

        [Fact]
        public async void SignIn_ThrowsExceptionFinding()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();

            User user = null;
            A.CallTo(() => fakeUserManager.FindByEmailAsync(A<string>._))
                .Returns(user);

            var userService = new UserService(fakeUserManager, null, null, null);

            await Assert.ThrowsAsync<ArgumentException>(() => userService.SignIn("email", "password"));
        }

        [Fact]
        public async void SignIn_ReturnsNull()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeSignInManager = A.Fake<SignInManager<User>>();

            A.CallTo(() => fakeUserManager.FindByEmailAsync(A<string>._))
                .Returns(new User());
            A.CallTo(() => fakeSignInManager.PasswordSignInAsync(A<string>._, A<string>._, A<bool>._, A<bool>._))
                .Returns(SignInResult.Failed);

            var userService = new UserService(fakeUserManager, fakeSignInManager, null, null);
            var result = await userService.SignIn("email", "password");

            Assert.Null(result);
        }

        [Fact]
        public async void GetByEmail_ReturnsUserDto()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeMapper = A.Fake<IMapper>();

            A.CallTo(() => fakeUserManager.FindByEmailAsync(A<string>._))
                .Returns(new User());

            var userService = new UserService(fakeUserManager, null, fakeMapper, null);
            var result = await userService.GetByEmail("email");

            Assert.IsAssignableFrom<UserDto>(result);
        }

        [Fact]
        public async void GetByEmail_ReturnsNull()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();

            User user = null;
            A.CallTo(() => fakeUserManager.FindByEmailAsync(A<string>._))
                .Returns(user);

            var userService = new UserService(fakeUserManager, null, null, null);

            var result = await userService.GetByEmail("email");
            Assert.Null(result);
        }

        [Fact]
        public async void GetById_ReturnsUserDtoFromCache()
        {
            var fakeCacheService = A.Fake<ICacheService>();

            A.CallTo(() => fakeCacheService.Get<UserDto>(A<string>._))
                .Returns(new UserDto());

            var userService = new UserService(null, null, null, fakeCacheService);
            var result = await userService.GetById("userId");

            Assert.IsAssignableFrom<UserDto>(result);
        }

        [Fact]
        public async void GetById_ReturnsUserDtoFromDb()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeMapper = A.Fake<IMapper>();
            var fakeCacheService = A.Fake<ICacheService>();

            UserDto userDto = null;
            A.CallTo(() => fakeCacheService.Get<UserDto>(A<string>._))
                .Returns(userDto);
            A.CallTo(() => fakeUserManager.FindByIdAsync(A<string>._))
                .Returns(new User());
            A.CallTo(() => fakeUserManager.GetRolesAsync(A<User>._))
                .Returns(new List<string>() { "User" });

            var userService = new UserService(fakeUserManager, null, fakeMapper, fakeCacheService);
            var result = await userService.GetById("email");

            Assert.IsAssignableFrom<UserDto>(result);
        }

        [Fact]
        public async void GetById_ThrowsException()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeCacheService = A.Fake<ICacheService>();

            UserDto userDto = null;
            A.CallTo(() => fakeCacheService.Get<UserDto>(A<string>._))
                .Returns(userDto);
            User user = null;
            A.CallTo(() => fakeUserManager.FindByIdAsync(A<string>._))
                .Returns(user);

            var userService = new UserService(fakeUserManager, null, null, fakeCacheService);

            await Assert.ThrowsAsync<ArgumentException>(async () => await userService.GetById("userId"));
        }

        [Fact]
        public async void UpdateUser_ReturnsUserDto_RemovingFromCacheIncluded()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeMapper = A.Fake<IMapper>();
            var fakeCacheService = A.Fake<ICacheService>();

            var user = A.Fake<User>();
            var userDto = A.Fake<UserDto>();
            A.CallTo(() => fakeUserManager.FindByIdAsync(A<string>._))
                .Returns(user);
            A.CallTo(() => fakeMapper.Map(A<UserModel>._, A<User>._))
                .Returns(user);
            A.CallTo(() => fakeUserManager.UpdateAsync(A<User>._))
                .Returns(IdentityResult.Success);
            A.CallTo(() => fakeCacheService.Get<UserDto>(A<string>._))
                .Returns(userDto);
            A.CallTo(() => fakeCacheService.RemoveFromCache(A<string>._))
                .DoesNothing();
            A.CallTo(() => fakeMapper.Map<UserDto>(A<User>._))
                .Returns(userDto);
            A.CallTo(() => fakeUserManager.GetRolesAsync(A<User>._))
                .Returns(new List<string>() { "User" });

            var userService = new UserService(fakeUserManager, null, fakeMapper, fakeCacheService);
            var result = await userService.UpdateUser("userId", new UserModel());

            Assert.Equal(HttpStatusCode.OK, result.resultCode);
        }

        [Fact]
        public async void UpdateUser_ReturnsUserDto_RemovingFromCacheNotIncluded()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeMapper = A.Fake<IMapper>();
            var fakeCacheService = A.Fake<ICacheService>();

            var user = A.Fake<User>();
            UserDto userDto = null;
            A.CallTo(() => fakeUserManager.FindByIdAsync(A<string>._))
                .Returns(user);
            A.CallTo(() => fakeMapper.Map(A<UserModel>._, A<User>._))
                .Returns(user);
            A.CallTo(() => fakeUserManager.UpdateAsync(A<User>._))
                .Returns(IdentityResult.Success);
            A.CallTo(() => fakeCacheService.Get<UserDto>(A<string>._))
                .Returns(userDto);
            A.CallTo(() => fakeMapper.Map<UserDto>(A<User>._))
                .Returns(new UserDto());
            A.CallTo(() => fakeUserManager.GetRolesAsync(A<User>._))
                .Returns(new List<string>() { "User" });

            var userService = new UserService(fakeUserManager, null, fakeMapper, fakeCacheService);
            var result = await userService.UpdateUser("userId", new UserModel());

            Assert.Equal(HttpStatusCode.OK, result.resultCode);
        }

        [Fact]
        public async void UpdateUser_ThrowsExceptionFinding()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            User user = null;
            A.CallTo(() => fakeUserManager.FindByIdAsync(A<string>._))
                .Returns(user);

            var userService = new UserService(fakeUserManager, null, null, null);
            var result = await userService.UpdateUser("userId", new UserModel());

            Assert.Equal(HttpStatusCode.Unauthorized, result.resultCode);
        }

        [Fact]
        public async void UpdateUser_ThrowsExceptionUpdating()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var fakeMapper = A.Fake<IMapper>();

            var user = A.Fake<User>();
            A.CallTo(() => fakeUserManager.FindByIdAsync(A<string>._))
                .Returns(user);
            A.CallTo(() => fakeMapper.Map(A<UserModel>._, A<User>._))
                .Returns(user);
            A.CallTo(() => fakeUserManager.UpdateAsync(A<User>._))
                .Returns(IdentityResult.Failed());

            var userService = new UserService(fakeUserManager, null, fakeMapper, null);
            var result = await userService.UpdateUser("userId", new UserModel());

            Assert.Equal(HttpStatusCode.BadRequest, result.resultCode);
        }

        [Fact]
        public async void UpdatePassword_ReturnsSuccess()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var user = A.Fake<User>();

            A.CallTo(() => fakeUserManager.FindByIdAsync(A<string>._))
                .Returns(user);
            A.CallTo(() => fakeUserManager.ChangePasswordAsync(A<User>._, A<string>._, A<string>._))
                .Returns(IdentityResult.Success);

            var userService = new UserService(fakeUserManager, null, null, null);
            var result = await userService.UpdatePassword("userId", new PasswordUpdateModel());

            Assert.Equal(HttpStatusCode.OK, result.resultCode);
        }

        [Fact]
        public async void UpdatePassword_ThrowsExceptionFinding()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            User user = null;

            A.CallTo(() => fakeUserManager.FindByIdAsync(A<string>._))
                .Returns(user);

            var userService = new UserService(fakeUserManager, null, null, null);
            var result = await userService.UpdatePassword("userId", new PasswordUpdateModel());

            Assert.Equal(HttpStatusCode.Unauthorized, result.resultCode);
        }

        [Fact]
        public async void UpdatePassword_ThrowsExceptionChanging()
        {
            var fakeUserManager = A.Fake<UserManager<User>>();
            var user = A.Fake<User>();

            A.CallTo(() => fakeUserManager.FindByIdAsync(A<string>._))
                .Returns(user);
            A.CallTo(() => fakeUserManager.ChangePasswordAsync(A<User>._, A<string>._, A<string>._))
                .Returns(IdentityResult.Failed());

            var userService = new UserService(fakeUserManager, null, null, null);
            var result = await userService.UpdatePassword("userId", new PasswordUpdateModel());

            Assert.Equal(HttpStatusCode.BadRequest, result.resultCode);
        }
    }
}