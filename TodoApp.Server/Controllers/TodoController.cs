using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Server.Interfaces;
using TodoApp.Server.Models;

namespace TodoApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Secure the controller with JWT authentication
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        /// <summary>
        /// Fetches all TODO items.
        /// </summary>
        /// <returns>List of TODO items.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodosAsync()
        {
            try
            {
                _logger.LogInformation("Received request to fetch all todos.");
                var todos = await _todoService.GetTodosAsync();
                return Ok(todos);  // Returns 200 OK with the list of todos
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching todos");
                return StatusCode(500, new ErrorResponse { Message = "Internal server error" });  // Returns 500 Internal Server Error with a message
            }
        }

        /// <summary>
        /// Fetches a specific TODO item by ID.
        /// </summary>
        /// <param name="id">ID of the TODO item.</param>
        /// <returns>The TODO item.</returns>
        [HttpGet("{id}")]
        [ActionName(nameof(GetTodoByIdAsync))]
        public async Task<ActionResult<Todo>> GetTodoByIdAsync(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound(new ErrorResponse { Message = $"Todo with ID {id} not found." });  // Returns 404 Not Found with a message
            }
            return Ok(todo);  // Returns 200 OK with the todo item
        }

        /// <summary>
        /// Adds a new TODO item.
        /// </summary>
        /// <param name="todo">The TODO item to add.</param>
        /// <returns>The added TODO item.</returns>
        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodoItemAsync(Todo todo)
        {
            try
            {
                _logger.LogInformation($"Received request to add a new todo: {todo.Title}");
                var createdTodo = await _todoService.AddTodoAsync(todo);

                if (createdTodo == null)
                {
                    return Conflict(new ErrorResponse { Message = $"A TODO item with the title '{todo.Title}' already exists." });  // Returns 409 Conflict with a message
                }

                return CreatedAtAction(nameof(GetTodoByIdAsync), new { id = createdTodo.Id }, createdTodo);  // Returns 201 Created with the newly created TODO item
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding todo");
                return StatusCode(500, new ErrorResponse { Message = "Internal server error" });  // Returns 500 Internal Server Error with a message
            }
        }

        /// <summary>
        /// Updates an existing TODO item.
        /// </summary>
        /// <param name="id">ID of the TODO item to update.</param>
        /// <param name="todo">The updated TODO item data.</param>
        /// <returns>No content if successful, or an error message.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTodoItemAsync(int id, Todo todo)
        {
            try
            {
                _logger.LogInformation($"Received request to update todo with ID: {id}");
                var updatedTodo = await _todoService.UpdateTodoAsync(id, todo);
                if (updatedTodo == null)
                {
                    _logger.LogWarning($"Todo with ID: {id} not found for update");
                    return NotFound(new ErrorResponse { Message = $"Todo with ID {id} not found." });  // Returns 404 Not Found with a message
                }
                return NoContent();  // Returns 204 No Content indicating the update was successful
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating todo with ID: {id}");
                return StatusCode(500, new ErrorResponse { Message = "Internal server error" });  // Returns 500 Internal Server Error with a message
            }
        }

        /// <summary>
        /// Deletes a TODO item by ID.
        /// </summary>
        /// <param name="id">ID of the TODO item to delete.</param>
        /// <returns>No content if successful, or an error message.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItemAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Received request to delete todo with ID: {id}");
                var todo = await _todoService.GetTodoByIdAsync(id);
                if (todo == null)
                {
                    _logger.LogWarning($"Todo with ID: {id} not found for deletion.");
                    return NotFound(new ErrorResponse { Message = $"Todo with ID {id} not found." });  // Returns 404 Not Found with a message
                }

                await _todoService.DeleteTodoAsync(id);
                return NoContent();  // Returns 204 No Content indicating the deletion was successful
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting todo with ID: {id}");
                return StatusCode(500, new ErrorResponse { Message = "Internal server error" });  // Returns 500 Internal Server Error with a message
            }
        }
    }
}
