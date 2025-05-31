namespace LYRA.Server.Models.Identity
{
    /// <summary>
    /// Represents a user registration request containing email and password details.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// User's email address for registration.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// User's chosen password.
        /// </summary>
        public string Password { get; set; } = default!;

        /// <summary>
        /// Confirmation of the user's chosen password to ensure accuracy.
        /// </summary>
        public string ConfirmPassword { get; set; } = default!;
    }
}
