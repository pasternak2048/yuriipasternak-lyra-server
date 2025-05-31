using LYRA.Server.Enums;
using LYRA.Server.Models.Verify;

namespace LYRA.Server.Services.Interfaces
{
    public interface ISignatureStringBuilder
    {
        AccessContext Context { get; }

        string BuildStringToSign(VerifyRequest request);
    }
}
