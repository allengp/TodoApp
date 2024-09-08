using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Server.Controllers;
using TodoApp.Server.Interfaces;
using TodoApp.Server.Models;

namespace TodoApp.Server.Tests.Controllers
{
    public class TodoControllerTests
    {
        private readonly Mock<ITodoService> _mockTodoService;
        private readonly Mock<ILogger<TodoController>> _mockLogger;
        private readonly TodoController _controller;

        public TodoControllerTests()
        {
            _mockTodoService = new Mock<ITodoService>();
            _mockLogger = new Mock<ILogger<TodoController>>();
            _controller = new TodoController(_mockTodoService.Object, _mockLogger.Object);
        }

        #region GetTodosAsync Tests

        [Fact]
        public async Task GetTodosAsync_ReturnsOkResult_WithListOfTodos()
        {
            // Arrange
            var todos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Todo 1", IsComplete = false },
                new Todo { Id = 2, Title = "Todo 2", IsComplete = true }
            };
            _mockTodoService.Setup(service => service.GetTodosAsync()).ReturnsAsync(todos);

            // Act
            var result = await _controller.GetTodosAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodos = Assert.IsType<List<Todo>>(okResult.Value);
            Assert.Equal(2, returnedTodos.Count);            
        }

        [Fact]
        public async Task GetTodosAsync_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockTodoService.Setup(service => service.GetTodosAsync()).ThrowsAsync(new System.Exception("Test Exception"));

            // Act
            var result = await _controller.GetTodosAsync();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", ((ErrorResponse)statusCodeResult.Value).Message);
        }

        #endregion

        #region GetTodoByIdAsync Tests

        [Fact]
        public async Task GetTodoByIdAsync_ReturnsOkResult_WhenTodoExists()
        {
            // Arrange
            var todo = new Todo { Id = 1, Title = "Todo 1", IsComplete = false };
            _mockTodoService.Setup(service => service.GetTodoByIdAsync(1)).ReturnsAsync(todo);

            // Act
            var result = await _controller.GetTodoByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodo = Assert.IsType<Todo>(okResult.Value);
            Assert.Equal(1, returnedTodo.Id);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockTodoService.Setup(service => service.GetTodoByIdAsync(1)).ReturnsAsync((Todo)null);

            // Act
            var result = await _controller.GetTodoByIdAsync(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Todo with ID 1 not found.", ((ErrorResponse)notFoundResult.Value).Message);
        }

        #endregion

        #region AddTodoItemAsync Tests

        [Fact]
        public async Task AddTodoItemAsync_ReturnsCreatedAtAction_WhenTodoIsAddedSuccessfully()
        {
            // Arrange
            var todo = new Todo { Id = 1, Title = "Test Todo", IsComplete = false };
            _mockTodoService.Setup(service => service.AddTodoAsync(It.IsAny<Todo>())).ReturnsAsync(todo);

            // Act
            var result = await _controller.AddTodoItemAsync(todo);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTodo = Assert.IsType<Todo>(createdAtActionResult.Value);
            Assert.Equal("Test Todo", returnedTodo.Title);
            Assert.Equal(1, returnedTodo.Id);
        }

        [Fact]
        public async Task AddTodoItemAsync_ReturnsConflict_WhenTodoWithSameTitleExists()
        {
            // Arrange
            var todo = new Todo { Title = "Test Todo", IsComplete = false };
            _mockTodoService.Setup(service => service.AddTodoAsync(It.IsAny<Todo>())).ReturnsAsync((Todo)null);

            // Act
            var result = await _controller.AddTodoItemAsync(todo);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            var errorResponse = Assert.IsType<ErrorResponse>(conflictResult.Value);
            Assert.Equal($"A TODO item with the title 'Test Todo' already exists.", errorResponse.Message);
        }

        [Fact]
        public async Task AddTodoItemAsync_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var todo = new Todo { Title = "Test Todo", IsComplete = false };
            _mockTodoService.Setup(service => service.AddTodoAsync(It.IsAny<Todo>())).ThrowsAsync(new System.Exception("Test Exception"));

            // Act
            var result = await _controller.AddTodoItemAsync(todo);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", ((ErrorResponse)statusCodeResult.Value).Message);
        }

        #endregion

        #region UpdateTodoItemAsync Tests

        [Fact]
        public async Task UpdateTodoItemAsync_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var todo = new Todo { Id = 1, Title = "Updated Todo", IsComplete = true };
            _mockTodoService.Setup(service => service.UpdateTodoAsync(1, It.IsAny<Todo>())).ReturnsAsync(todo);

            // Act
            var result = await _controller.UpdateTodoItemAsync(1, todo);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateTodoItemAsync_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            var todo = new Todo { Id = 1, Title = "Updated Todo", IsComplete = true };
            _mockTodoService.Setup(service => service.UpdateTodoAsync(1, It.IsAny<Todo>())).ReturnsAsync((Todo)null);

            // Act
            var result = await _controller.UpdateTodoItemAsync(1, todo);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Equal($"Todo with ID 1 not found.", errorResponse.Message);
        }

        #endregion

        #region DeleteTodoItemAsync Tests

        [Fact]
        public async Task DeleteTodoItemAsync_ReturnsNoContent_WhenTodoIsDeletedSuccessfully()
        {
            // Arrange
            _mockTodoService.Setup(service => service.GetTodoByIdAsync(1)).ReturnsAsync(new Todo { Id = 1, Title = "Todo 1" });
            _mockTodoService.Setup(service => service.DeleteTodoAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTodoItemAsync(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTodoItemAsync_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockTodoService.Setup(service => service.GetTodoByIdAsync(1)).ReturnsAsync((Todo)null);

            // Act
            var result = await _controller.DeleteTodoItemAsync(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Equal($"Todo with ID 1 not found.", errorResponse.Message);
        }

        #endregion
    }
}
