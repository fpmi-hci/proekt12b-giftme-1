using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WishList.WebApi.Auth;

namespace WishList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public BaseController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        protected string GetCurrentUserId(string token)
        {
            var jwtToken = _authManager.DecodeJwtToken(token);
            var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

            return userId;
        }
    }
}
