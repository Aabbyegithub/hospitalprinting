using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebServiceClass.Base
{
    public class TokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(SymmetricSecurityKey key)
        {

            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public string GenerateToken(string username)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Role, "User") // 示例角色
        };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(7);
            var token = new JwtSecurityToken(
                signingCredentials: creds,
                claims: claims,
                expires: expires);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
