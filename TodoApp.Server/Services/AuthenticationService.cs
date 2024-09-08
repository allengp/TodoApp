using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApp.Server.Interfaces;
using TodoApp.Server.Settings;

namespace TodoApp.Server.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IDictionary<string, string> _users = new Dictionary<string, string>
        {
            { "testuser", "password" }
        };

        public AuthenticationService(JwtSettings jwtSettings, ILogger<AuthenticationService> logger)
        {
            _jwtSettings = jwtSettings;
            _logger = logger;
        }

        public string Authenticate(string username, string password)
        {
            // Check if the user credentials are correct
            if (!_users.ContainsKey(username) || _users[username] != password)
            {
                _logger.LogWarning($"Authentication failed for username: {username}");
                return null;
            }

            _logger.LogInformation($"User {username} authenticated successfully.");

            try
            {
                // Create the token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, username)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),  // Use ExpiryInMinutes
                    Audience = _jwtSettings.Audience,
                    Issuer = _jwtSettings.Issuer,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                _logger.LogInformation($"Token generated for user {username}.");
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while generating token for user {username}.");
                throw;
            }
        }
    }
}
