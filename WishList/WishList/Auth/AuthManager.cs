using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WishList.Business.ModelsDto;

namespace WishList.WebApi.Auth
{
    public class AuthManager : IAuthManager
    {
        private readonly IConfiguration _configuration;

        public AuthManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserDto userDto, DateTime now)
        {
            if (userDto == null)
            {
                throw new NullReferenceException("UserDto is null");
            }

            var claims = new Claim[]
            {
                new("id", userDto.Id),
                new("email", userDto.Email),
                new(ClaimTypes.Role, userDto.Role),
            };

            var jwtToken = new JwtSecurityToken(_configuration.GetValue<string>("JsonWebToken:Issuer"),
                claims: claims,
                expires: now.AddMinutes(60),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JsonWebToken:SecretKey"))), SecurityAlgorithms.HmacSha256));
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }

        public JwtSecurityToken DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }

            try

            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token,
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = _configuration.GetValue<string>("JsonWebToken:Issuer"),
                            ValidateAudience = false,
                            RequireExpirationTime = true,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,
                            RequireSignedTokens = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                                _configuration.GetValue<string>("JsonWebToken:SecretKey")))
                        },
                        out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return jwtToken;
            }
            catch
            {
                return null;
            }
        }
    }
}

