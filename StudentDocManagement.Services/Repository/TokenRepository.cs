using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;

namespace StudentDocManagement.Services.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;

        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Generate JWT token for a user with roles
        public Task<string> GetToken(ApplicationUser user, IList<string> roles)
        {
            // Create a list of claims that will be embedded in the token
            var authClaims = new List<Claim>
        {
           new Claim(ClaimTypes.Name, user.UserName),// User's username
                new Claim("FullName", user.FullName ?? string.Empty),// Optional full name claim
                new Claim(ClaimTypes.NameIdentifier, user.Id),// User ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
           

        };

            // Add roles as claims
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            // Get secret key from configuration and create symmetric security key
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            // Create the JWT token
            var token = new JwtSecurityToken(
                 issuer: _configuration["Jwt:Issuer"],// Issuer of the token
                audience: _configuration["Jwt:Audience"],// Audience for the token
                expires: DateTime.Now.AddHours(3),// Token expiry (3 hours)
                claims: authClaims,// Claims to include
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256) // Signing credentials
            );
            // Convert token object to string
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            // Return token as Task<string>
            return Task.FromResult(tokenString);
        }
    }
}
