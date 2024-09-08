using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Server.Controllers;
using TodoApp.Server.Interfaces;
using TodoApp.Server.Models;

namespace TodoApp.Server.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;

        public AuthControllerTests()
        {
            // Arrange - Set up mocks and controller
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Authenticate_ReturnsBadRequest_WhenUsernameOrPasswordIsMissing()
        {
            // Arrange
            // Arrange
            var user = new User { Username = "", Password = "password" };  // Missing username

            // Act
            var result = _authController.Authenticate(user);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Ensure BadRequest is returned
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);  // Ensure ErrorResponse type
            Assert.Equal("Username and password are required.", errorResponse.Message);
        }

        [Fact]
        public void Authenticate_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "wrongpassword" };  // Invalid password
            _mockAuthService.Setup(service => service.Authenticate(user.Username, user.Password)).Returns((string)null);

            // Act
            var result = _authController.Authenticate(user);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);  // Ensure Unauthorized is returned
            var errorResponse = Assert.IsType<ErrorResponse>(unauthorizedResult.Value);  // Ensure ErrorResponse type
            Assert.Equal("Invalid username or password.", errorResponse.Message);            
        }

        [Fact]
        //public void Authenticate_ReturnsOkResult_WhenValidCredentials()
        //{
        //    // Arrange
        //    var user = new User { Username = "testuser", Password = "password" };  // Valid credentials
        //    var expectedToken = "valid_token";
        //    _mockAuthService.Setup(service => service.Authenticate(user.Username, user.Password)).Returns(expectedToken);

        //    // Act
        //    var result = _authController.Authenticate(user);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);  // Ensure Ok is returned
        //    dynamic tokenResponse = okResult.Value;
        //    Assert.Equal(expectedToken, tokenResponse.Token);
        //}
        public void Authenticate_ReturnsOkResult_WhenValidCredentials()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "password" };  // Valid credentials
            var expectedToken = "valid_token";
            _mockAuthService.Setup(service => service.Authenticate(user.Username, user.Password)).Returns(expectedToken);

            // Act
            var result = _authController.Authenticate(user);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);  // Ensure Ok is returned

            // Use dynamic to access the anonymous object and assert the token value
            dynamic tokenResponse = okResult.Value;
            Assert.Equal(expectedToken, (string)tokenResponse.Token);  // Cast the token to string
        }

        [Fact]
        public void Authenticate_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "password" };
            _mockAuthService.Setup(service => service.Authenticate(user.Username, user.Password))
                .Throws(new Exception("Test Exception"));

            // Act
            var result = _authController.Authenticate(user);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);  // Ensure StatusCode is returned
            Assert.Equal(500, statusCodeResult.StatusCode);  // Ensure Internal Server Error (500)
            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);  // Ensure ErrorResponse type
            Assert.Equal("Internal server error", errorResponse.Message);
        }
    }
}
