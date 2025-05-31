namespace LYRA.Server.Models.Identity
{
    /// <summary>
    /// Represents a login request containing user credentials.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// User's email address used for authentication.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// User's password used for authentication.
        /// </summary>
        public string Password { get; set; } = default!;
    }
}
