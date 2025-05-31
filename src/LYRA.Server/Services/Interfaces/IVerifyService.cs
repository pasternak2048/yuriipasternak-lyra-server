using LYRA.Server.Models.Verify;

namespace LYRA.Server.Services.Interfaces
{
    public interface IVerifyService
    {
        Task<VerifyResponse> Verify(VerifyRequest request);
    }
}
