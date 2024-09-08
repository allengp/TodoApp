using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Server.Models;
using TodoApp.Server.Services;

namespace TodoApp.Server.Tests.Services
{
    public class TodoServiceTests
    {
        private readonly TodoService _todoService;
        private readonly Mock<ILogger<TodoService>> _mockLogger;

        public TodoServiceTests()
        {
            // Setup a mock logger
            _mockLogger = new Mock<ILogger<TodoService>>();

            // Initialize the service with the mock logger
            _todoService = new TodoService(_mockLogger.Object);
        }

        [Fact]
        public async Task GetTodosAsync_ShouldReturnAllTodos()
        {
            // Act
            var result = await _todoService.GetTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count()); // Assuming the initial default todos are 3
        }

        [Fact]
        public async Task GetTodoByIdAsync_ShouldReturnTodo_WhenTodoExists()
        {
            // Act
            var result = await _todoService.GetTodoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Wash clothes", result.Title);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ShouldReturnNull_WhenTodoDoesNotExist()
        {
            // Act
            var result = await _todoService.GetTodoByIdAsync(999); // ID that doesn't exist

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddTodoAsync_ShouldAddTodo_WhenTodoDoesNotExist()
        {
            // Arrange
            var newTodo = new Todo { Title = "Test new todo", IsComplete = false };

            // Act
            var result = await _todoService.AddTodoAsync(newTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Id); // New todo ID should be 4 (since there are 3 initial todos)
            Assert.Equal("Test new todo", result.Title);
        }

        [Fact]
        public async Task AddTodoAsync_ShouldReturnNull_WhenTodoWithSameTitleExists()
        {
            // Arrange
            var duplicateTodo = new Todo { Title = "Wash clothes", IsComplete = false }; // Duplicate title

            // Act
            var result = await _todoService.AddTodoAsync(duplicateTodo);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTodoAsync_ShouldUpdateTodo_WhenTodoExists()
        {
            // Arrange
            var updatedTodo = new Todo { Title = "Updated Title", IsComplete = true };

            // Act
            var result = await _todoService.UpdateTodoAsync(1, updatedTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
            Assert.True(result.IsComplete);
        }

        [Fact]
        public async Task UpdateTodoAsync_ShouldReturnNull_WhenTodoDoesNotExist()
        {
            // Arrange
            var updatedTodo = new Todo { Title = "Non-existing Todo", IsComplete = true };

            // Act
            var result = await _todoService.UpdateTodoAsync(999, updatedTodo); // Non-existent ID

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteTodoAsync_ShouldDeleteTodo_WhenTodoExists()
        {
            // Act
            await _todoService.DeleteTodoAsync(1);

            // Assert
            var result = await _todoService.GetTodoByIdAsync(1);
            Assert.Null(result);
        }
        
    }
}
