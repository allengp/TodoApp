namespace TodoApp.Server.Interfaces
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates the user based on their username and password.
        /// </summary>
        /// <param name="username">The username of the user attempting to authenticate.</param>
        /// <param name="password">The password of the user attempting to authenticate.</param>
        /// <returns>A JWT token if authentication is successful; otherwise, null if the authentication fails.</returns>
        string Authenticate(string username, string password);
    }
}
