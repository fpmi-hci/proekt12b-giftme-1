using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;
using WishList.Business.IServices;
using WishList.Business.Models;
using WishList.Business.ModelsDto;
using WishList.WebApi.Auth;
using WishList.WebApi.Controllers;
using WishList.Business.Tools;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WishList.UnitTests.Controllers
{
    public class UserControllerTests
    {
        private static readonly UserController _userController;
        private static readonly IAuthManager _authManager;

        static UserControllerTests()
        {
            _authManager = A.Fake<IAuthManager>();

            var fakeUserService = A.Fake<IUserService>();
            var fakeMapper = A.Fake<IMapper>();
            var fakeServiceProvider = A.Fake<IServiceProvider>();
            var fakeAuthService = A.Fake<IAuthenticationService>();
            var fakeTicket = A.Fake<AuthenticationTicket>();
            var fakeContext = A.Fake<HttpContext>();

            A.CallTo(() => fakeUserService.UpdateUser(A<string>._, A<UserModel>._))
                 .Returns(new RequestResult { resultCode = HttpStatusCode.OK});
            A.CallTo(() => fakeUserService.UpdatePassword(A<string>._, A<PasswordUpdateModel>._))
                .Returns(new RequestResult());
            A.CallTo(() => fakeMapper.Map<UserModel>(A<UserDto>._))
                .Returns(new UserModel());
            A.CallTo(() => _authManager.DecodeJwtToken(A<string>._))
                .Returns(new JwtSecurityToken());
            A.CallTo(() => fakeContext.RequestServices)
                .Returns(fakeServiceProvider);
            A.CallTo(() => fakeServiceProvider.GetService(typeof(IAuthenticationService)))
                .Returns(fakeAuthService);
            A.CallTo(() => fakeAuthService.AuthenticateAsync(fakeContext, A<string>._))
                .Returns(AuthenticateResult.Success(fakeTicket));

            _userController = new UserController(fakeUserService, fakeMapper, _authManager)
            {
                ControllerContext = { HttpContext = fakeContext }
            };
        }

        [Fact]
        public async void UpdateUser_ReturnUpdatedModel()
        {
            StubAuthManager(Guid.NewGuid().ToString());

            var result = await _userController.UpdateUser(new UserModel());

            var okResult = result as ObjectResult;
            Xunit.Assert.NotNull(okResult);
            Xunit.Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async void UpdateUser_ReturnNoauthorized()
        {
            StubAuthManager(string.Empty);

            var result = await _userController.UpdateUser(new UserModel());

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Xunit.Assert.NotNull(unauthorizedResult);
            Xunit.Assert.IsType<string>(unauthorizedResult.Value);
            Xunit.Assert.Equal("User not found.", unauthorizedResult.Value);
        }

        [Fact]
        public async void UpdateUser_ReturnBadRequest()
        {
            StubAuthManager(Guid.NewGuid().ToString());

            var result = await _userController.UpdateUser(null);

            var badRequestResult = result as BadRequestObjectResult;
            Xunit.Assert.NotNull(badRequestResult);
            Xunit.Assert.IsType<string>(badRequestResult.Value);
            Xunit.Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Xunit.Assert.Equal("Not enough parameters to update user.", badRequestResult.Value);
        }

        [Fact]
        public async void UpdatePassword_ReturnUpdatedPassword()
        {
            StubAuthManager(Guid.NewGuid().ToString());

            var result = await _userController.UpdatePassword(new PasswordUpdateModel());

            var okResult = result as ObjectResult;
            Xunit.Assert.NotNull(okResult);
            Xunit.Assert.Equal(0, okResult.StatusCode);
        }

        [Fact]
        public async void UpdatePassword_ReturnBadRequest()
        {
            StubAuthManager(Guid.NewGuid().ToString());

            var result = await _userController.UpdatePassword(null);

            var badRequestResult = result as BadRequestObjectResult;
            Xunit.Assert.NotNull(badRequestResult);
            Xunit.Assert.IsType<string>(badRequestResult.Value);
            Xunit.Assert.Equal("Not enough parameters to update password.", badRequestResult.Value);
        }

        [Fact]
        public async void GetUserInfo_ReturnUserModel()
        {
            StubAuthManager(Guid.NewGuid().ToString());

            var result = await _userController.GetUserInfo();

            var okResult = result as OkObjectResult;
            Xunit.Assert.NotNull(okResult);
            Xunit.Assert.IsType<UserModel>(okResult.Value);
            Xunit.Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async void GetUserInfo_ReturnBadRequest()
        {
            StubAuthManager(string.Empty);

            var result = await _userController.GetUserInfo();

            var unauthorizedResult = result as UnauthorizedObjectResult;
            Xunit.Assert.NotNull(unauthorizedResult);
            Xunit.Assert.IsType<string>(unauthorizedResult.Value);
            Xunit.Assert.Equal("User not found.", unauthorizedResult.Value);
        }


        #region Helpers

        private void StubAuthManager(string userId)
        {
            var jwtToken = new JwtSecurityToken(claims: new Claim[]
            {
                new("id", userId)
            });

            A.CallTo(() => _authManager.DecodeJwtToken(A<string>._))
                .Returns(jwtToken);
        }

        #endregion
    }
}
