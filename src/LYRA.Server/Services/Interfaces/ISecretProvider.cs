using LYRA.Server.Entities;

namespace LYRA.Server.Services.Interfaces
{
    public interface ISecretProvider
    {
        Task<TrustedTouchpointEntity?> GetTouchpointAsync(string systemName);
    }
}
