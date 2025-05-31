namespace LYRA.Server.Services.Interfaces
{
    /// <summary>
    /// Service interface for handling user identity operations such as login, registration, and logout.
    /// </summary>
    public interface IIdentityService
    {
        /// <summary>
        /// Attempts to log in a user with the specified email and password.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password for the account.</param>
        /// <returns>
        /// A tuple indicating whether the login was successful and an optional error message.
        /// </returns>
        Task<(bool Success, string? Error)> LoginAsync(string email, string password);

        /// <summary>
        /// Attempts to register a new user account with the specified credentials.
        /// </summary>
        /// <param name="email">The email address for the new account.</param>
        /// <param name="password">The chosen password for the account.</param>
        /// <param name="confirmPassword">The password confirmation to validate against the password.</param>
        /// <returns>
        /// A tuple indicating whether the registration was successful and an optional error message.
        /// </returns>
        Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string confirmPassword);

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        Task LogoutAsync();
    }
}
