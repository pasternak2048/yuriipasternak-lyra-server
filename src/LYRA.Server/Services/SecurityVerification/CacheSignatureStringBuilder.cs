using LYRA.Server.Enums;
using LYRA.Server.Models.Verify;
using LYRA.Server.Services.Interfaces;

namespace LYRA.Server.Services.SecurityVerification
{
    public class CacheSignatureStringBuilder : ISignatureStringBuilder
    {
        public AccessContext Context => AccessContext.Cache;

        public string BuildStringToSign(VerifyRequest request)
        {
            return $"caller={request.Caller}&target={request.Target}&operation={request.Method}&key={request.Path}&payloadHash={request.PayloadHash}&timestamp={request.Timestamp}";
        }
    }
}
