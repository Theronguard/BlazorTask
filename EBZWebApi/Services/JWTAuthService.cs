using EBZShared.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EBZWebApi.Services
{
    /// <summary>
    /// Handles JWT Authorization
    /// </summary>
    public class JWTAuthService : IJWTAuthService
    {
        private readonly IConfiguration _configuration;

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public JWTAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT Token based on a username
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GenerateToken(User user)
        {
            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username)
            };

            string key = _configuration.GetValue<string>("JWTAuth:Key");
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            int tokenExpieration = _configuration.GetValue<int>("JWTAuth:TokenExpiration");
            string issuer = _configuration.GetValue<string>("JWTAuth:Issuer");
            string audience = _configuration.GetValue<string>("JWTAuth:Audience");

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpieration),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        #endregion
    }
}
