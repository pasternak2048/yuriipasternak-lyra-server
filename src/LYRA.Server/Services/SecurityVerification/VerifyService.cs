using LYRA.Security.Models.Verify;
using LYRA.Security.Signature;
using LYRA.Security.Utilities.Security;
using LYRA.Server.Services.Interfaces;

namespace LYRA.Server.Services.SecurityVerification
{
    /// <summary>
    /// Service responsible for verifying signed requests by validating the digital signature
    /// using cached access policy data for maximum performance.
    /// </summary>
    public class VerifyService : IVerifyService
    {
        private readonly SignatureStringBuilderFactory _factory;
        private readonly ICachedAccessPolicyService _cache;
        private readonly ILogger<VerifyService> _logger;

        public VerifyService(
            SignatureStringBuilderFactory factory,
            ICachedAccessPolicyService cache,
            ILogger<VerifyService> logger)
        {
            _factory = factory;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Performs verification using cached access policy with denormalized data.
        /// </summary>
        public async Task<VerifyResponse> Verify(VerifyRequest request)
        {
            try
            {
                var operationKey = $"{request.Method} {request.Path}".ToLowerInvariant();
                var policy = await _cache.FindAsync(
                    request.Caller, request.Target, request.Context.ToString(), operationKey);

                if (policy == null || !policy.IsEnabled)
                    return Failure($"Access denied for '{request.Caller}' to '{request.Target}' ({request.Context}).");

                // Validate payload hash if needed
                var metadata = SignatureContextRegistry.GetMetadata(request.Context);
                if (metadata.RequiresPayloadHash(request))
                {
                    if (string.IsNullOrWhiteSpace(request.Payload))
                        return Failure("Payload is required for this context and method.");

                    if (string.IsNullOrWhiteSpace(request.PayloadHash))
                        return Failure("PayloadHash is required for this context and method.");

                    var computed = EncryptionHelper.ComputeSha512(request.Payload);
                    if (!EncryptionHelper.SecureEquals(computed, request.PayloadHash))
                        return Failure("PayloadHash does not match payload.");
                }

                // Generate string to sign
                var builder = _factory.GetBuilder(request.Context);
                var stringToSign = builder.BuildStringToSign(
                    request.Caller, request.Target, request.Method, request.Path,
                    request.PayloadHash, request.Timestamp);

                // Decrypt and use secret
                var decryptedSecret = EncryptionHelper.DecryptSecret(policy.CallerSecret);
                var expectedSignature = EncryptionHelper.ComputeHmacSha512(stringToSign, decryptedSecret);
                var isValid = EncryptionHelper.SecureEquals(expectedSignature, request.Signature);

                _logger.LogInformation("Verification result for {Caller} → {Target}: {Result}",
                    request.Caller, request.Target, isValid ? "SUCCESS" : "FAILURE");

                return isValid ? VerifyResponse.Success : Failure("Invalid signature.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected verification failure");
                return Failure($"Verification failed: {ex.Message}");
            }
        }

        private static VerifyResponse Failure(string message) => new()
        {
            IsSuccess = false,
            ErrorMessage = message
        };
    }
}
