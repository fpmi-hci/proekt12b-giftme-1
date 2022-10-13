using System.IdentityModel.Tokens.Jwt;
using WishList.Business.ModelsDto;

namespace WishList.WebApi.Auth
{
    public interface IAuthManager
    {
        public string GenerateToken(UserDto userDto, DateTime now);
        public JwtSecurityToken DecodeJwtToken(string token);
    }
}

