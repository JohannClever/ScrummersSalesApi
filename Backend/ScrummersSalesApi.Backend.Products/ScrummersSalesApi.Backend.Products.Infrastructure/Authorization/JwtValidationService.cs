using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ScrummersSalesApi.Backend.Products.Infrastructure.Authorization
{
    public class JwtValidationService
    {
        private readonly IConfiguration _config;
        public JwtValidationService(IConfiguration config)
        {
            _config = config;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = JWTHelper.GetValidationParameters(_config);
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }
        
    }

    public static class JWTHelper
    {
        public static TokenValidationParameters GetValidationParameters(IConfiguration config)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"])),
            };
        }
    }
}
