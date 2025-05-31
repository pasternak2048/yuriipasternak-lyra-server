using LYRA.Server.Enums;
using LYRA.Server.Models.Verify;
using LYRA.Server.Services.Interfaces;

namespace LYRA.Server.Services.Verify
{
    public class HttpSignatureStringBuilder : ISignatureStringBuilder
    {
        public AccessContext Context => AccessContext.Http;

        public string BuildStringToSign(VerifyRequest request)
        {
            return $"caller={request.Caller}&target={request.Target}&method={request.Method}&path={request.Path}&payloadHash={request.PayloadHash}&timestamp={request.Timestamp}";
        }
    }
}
