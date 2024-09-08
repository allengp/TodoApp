using TodoApp.Server.Models;

namespace TodoApp.Server.Interfaces
{
    public interface ITodoService
    {
        /// <summary>
        /// Retrieves all TODO items.
        /// </summary>
        /// <returns>An enumerable list of TODO items.</returns>
        Task<IEnumerable<Todo>> GetTodosAsync();  // Method to get all TODO items asynchronously.

        /// <summary>
        /// Retrieves a TODO item by its ID.
        /// </summary>
        /// <param name="id">The ID of the TODO item.</param>
        /// <returns>The TODO item with the specified ID, or null if not found.</returns>
        Task<Todo> GetTodoByIdAsync(int id);  // Method to get a specific TODO item by its ID asynchronously.

        /// <summary>
        /// Adds a new TODO item.
        /// </summary>
        /// <param name="todo">The TODO item to add.</param>
        /// <returns>The added TODO item.</returns>
        Task<Todo> AddTodoAsync(Todo todo);  // Method to add a new TODO item asynchronously.

        /// <summary>
        /// Updates an existing TODO item by its ID.
        /// </summary>
        /// <param name="id">The ID of the TODO item to update.</param>
        /// <param name="todo">The updated TODO item details.</param>
        /// <returns>The updated TODO item, or null if not found.</returns>
        Task<Todo> UpdateTodoAsync(int id, Todo todo);  // Method to update an existing TODO item by its ID asynchronously.

        /// <summary>
        /// Deletes a TODO item by its ID.
        /// </summary>
        /// <param name="id">The ID of the TODO item to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteTodoAsync(int id);  // Method to delete a TODO item by its ID asynchronously.
    }
}

