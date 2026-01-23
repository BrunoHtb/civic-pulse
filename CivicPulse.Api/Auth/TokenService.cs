using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CivicPulse.Api.Auth
{
    public class TokenService
    {
        private readonly JwtOptions _opts;

        public TokenService(IOptions<JwtOptions> opts)
        {
            _opts = opts.Value;
        }

        public (string token, DateTime expirtesAtUtc) CreateAdminToken(string username)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_opts.ExpiryMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, username),
                new(ClaimTypes.Role, "admin"),
                new("scope", "ingestion")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _opts.Issuer,
                audience: _opts.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return (token, expires);
        }
    }
}
