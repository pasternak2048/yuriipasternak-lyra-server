namespace LYRA.Server.Models.Verify
{
    /// <summary>
    /// Represents the result of a verification attempt for a signed request.
    /// </summary>
    public class VerifyResponse
    {
        /// <summary>
        /// Indicates whether the signature is valid and verification passed.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Optional error message describing why verification failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// HTTP-like status code: 200 if successful, 403 if verification failed.
        /// </summary>
        public int StatusCode => IsSuccess ? 200 : 403;

        /// <summary>
        /// Predefined success response to avoid allocation.
        /// </summary>
        public static readonly VerifyResponse Success = new()
        {
            IsSuccess = true,
            ErrorMessage = null
        };
    }
}
