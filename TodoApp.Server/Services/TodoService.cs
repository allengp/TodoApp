using TodoApp.Server.Interfaces;
using TodoApp.Server.Models;

namespace TodoApp.Server.Services
{
    public class TodoService : ITodoService
    {
        private readonly List<Todo> _todos = new();
        private readonly ILogger<TodoService> _logger;

        public TodoService(ILogger<TodoService> logger)
        {
            _logger = logger;

            // Initialize with default data
            _todos.AddRange(new List<Todo>
            {
                new Todo { Id = 1, Title = "Wash clothes", IsComplete = false },
                new Todo { Id = 2, Title = "Vacuum Carpet", IsComplete = true },
                new Todo { Id = 3, Title = "Water the garden", IsComplete = false }
            });

            _logger.LogInformation("Initialized with default TODO data");
        }

        public async Task<IEnumerable<Todo>> GetTodosAsync()
        {
            _logger.LogInformation("Fetching all TODO items");
            return await Task.FromResult(_todos);
        }

        public async Task<Todo> GetTodoByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching TODO item with ID: {id}");
            var todo = _todos.FirstOrDefault(t => t.Id == id);

            if (todo == null)
            {
                _logger.LogWarning($"TODO item with ID {id} not found");
            }

            return await Task.FromResult(todo);
        }

        public async Task<Todo> AddTodoAsync(Todo todo)
        {
            try
            {
                // Check if a todo item with the same title already exists
                var existingTodo = _todos.FirstOrDefault(t => t.Title.Equals(todo.Title, StringComparison.OrdinalIgnoreCase));

                if (existingTodo != null)
                {
                    _logger.LogWarning($"TODO item with Title: {todo.Title} already exists. Cannot add duplicate.");
                    return null; // You could also throw an exception if you prefer
                }

                // Ensure the new Todo has a unique Id
                todo.Id = _todos.Any() ? _todos.Max(t => t.Id) + 1 : 1;

                _todos.Add(todo);
                _logger.LogInformation($"TODO item added successfully with ID: {todo.Id} and Title: {todo.Title}");
                return await Task.FromResult(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding TODO item");
                throw;
            }
        }

        public async Task<Todo> UpdateTodoAsync(int id, Todo todo)
        {
            try
            {
                var existingTodo = await GetTodoByIdAsync(id);
                if (existingTodo == null)
                {
                    _logger.LogWarning($"TODO item with ID {id} not found for update");
                    return null;
                }

                existingTodo.Title = todo.Title;
                existingTodo.IsComplete = todo.IsComplete;

                _logger.LogInformation($"TODO item with ID {id} updated successfully with new Title: {todo.Title}");
                return await Task.FromResult(existingTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while updating TODO item with ID: {id}");
                throw;
            }
        }

        public async Task DeleteTodoAsync(int id)
        {
            try
            {
                var todo = await GetTodoByIdAsync(id);
                if (todo == null)
                {
                    _logger.LogWarning($"TODO item with ID {id} not found for deletion");
                    return;
                }

                _todos.Remove(todo);
                _logger.LogInformation($"TODO item with ID {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting TODO item with ID: {id}");
                throw;
            }
        }
    }
}
