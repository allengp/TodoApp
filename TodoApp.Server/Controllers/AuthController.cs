using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Server.Interfaces;
using TodoApp.Server.Models;

namespace TodoApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthenticationService authenticationService, ILogger<AuthController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate([FromBody] User user)
        {
            try
            {
                // Log the start of the authentication attempt
                _logger.LogInformation("Authentication attempt for user: {Username}", user?.Username);

                // Check for missing username or password
                if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                {
                    _logger.LogWarning("Invalid login attempt: Missing username or password.");
                    return BadRequest(new ErrorResponse { Message = "Username and password are required." });
                }

                _logger.LogInformation("Authenticating user: {Username}", user.Username);

                // Authenticate the user
                var token = _authenticationService.Authenticate(user.Username, user.Password);
                if (token == null)
                {
                    _logger.LogWarning("Authentication failed for user: {Username}", user.Username);
                    return Unauthorized(new ErrorResponse { Message = "Invalid username or password." });
                }

                _logger.LogInformation("User {Username} authenticated successfully.", user.Username);
                return Ok(new TokenResponse { Token = token });
            }
            catch (Exception ex)
            {
                // Log the error with exception details
                _logger.LogError(ex, "An error occurred during the authentication of user: {Username}", user?.Username);
                return StatusCode(500, new ErrorResponse { Message = "Internal server error" });
            }
        }
    }
}
