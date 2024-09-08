using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Server.Services;
using TodoApp.Server.Settings;

namespace TodoApp.Server.Tests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly AuthenticationService _authService;
        private readonly Mock<ILogger<AuthenticationService>> _mockLogger;

        public AuthenticationServiceTests()
        {
            // Set up JWT settings with valid values
            var jwtSettings = new JwtSettings
            {
                SecretKey = "ThisIsASecretKeyWithMoreThan32Characters!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryInMinutes = 30
            };

            // Set up the mock logger
            _mockLogger = new Mock<ILogger<AuthenticationService>>();

            // Create an instance of the AuthenticationService with mock dependencies
            _authService = new AuthenticationService(jwtSettings, _mockLogger.Object);
        }

        [Fact]
        public void Authenticate_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Act
            var token = _authService.Authenticate("testuser", "password");

            // Assert
            Assert.NotNull(token); // A token should be generated

            // Verify that LogInformation was called with the expected message for successful authentication
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("User testuser authenticated successfully.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            // Verify that LogInformation was called with the expected message for token generation
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Token generated for user testuser.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenUsernameIsInvalid()
        {
            // Act
            var token = _authService.Authenticate("invaliduser", "password");

            // Assert
            Assert.Null(token); // No token should be generated

            // Verify that LogWarning was called with the expected message
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Authentication failed for username: invaliduser")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Authenticate_ShouldReturnNull_WhenPasswordIsInvalid()
        {
            // Act
            var token = _authService.Authenticate("testuser", "wrongpassword");

            // Assert
            Assert.Null(token); // No token should be generated

            // Verify that LogWarning was called with the expected message
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Authentication failed for username: testuser")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Authenticate_ShouldLogError_WhenTokenGenerationFails()
        {
            // Arrange
            var mockJwtSettings = new JwtSettings
            {
                SecretKey = "", // Invalid key to trigger exception
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryInMinutes = 30
            };

            // Create a new instance of AuthenticationService with the invalid settings
            var authService = new AuthenticationService(mockJwtSettings, _mockLogger.Object);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authService.Authenticate("testuser", "password"));
            Assert.Contains("Cannot create a 'Microsoft.IdentityModel.Tokens.SymmetricSecurityKey'", exception.Message);

            // Verify that an error log was generated
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while generating token for user testuser.")),
                    It.IsAny<ArgumentException>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
