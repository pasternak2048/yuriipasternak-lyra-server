namespace LYRA.Server.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<(bool Success, string? Error)> LoginAsync(string email, string password);

        Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string confirmPassword);

        Task LogoutAsync();
    }
}
