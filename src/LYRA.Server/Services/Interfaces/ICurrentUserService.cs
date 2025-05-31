namespace LYRA.Server.Services.Interfaces
{
    /// <summary>
    /// Provides information about the currently authenticated user within the application context.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the ID of the currently authenticated user, if available.
        /// </summary>
        Guid? UserId { get; }
    }
}
