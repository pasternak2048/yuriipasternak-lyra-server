using LYRA.Security.Models.Verify;
using LYRA.Security.Signature;
using LYRA.Security.Utilities.Security;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.Verify.Interfaces;
using LYRA.Server.Utilities;
using System.Globalization;

namespace LYRA.Server.Services.Verify
{
    /// <summary>
    /// Service responsible for verifying signed requests by validating the digital signature
    /// using cached access policy data from in-memory cache for maximum performance.
    /// </summary>
    public class VerifyService : IVerifyService
    {
        private readonly SignatureStringBuilderFactory _factory;
        private readonly ICachedAccessPolicyMemoryService _memory;
        private readonly ILogger<VerifyService> _logger;

        public VerifyService(
            SignatureStringBuilderFactory factory,
            ICachedAccessPolicyMemoryService memory,
            ILogger<VerifyService> logger)
        {
            _factory = factory;
            _memory = memory;
            _logger = logger;
        }

        /// <summary>
        /// Performs verification using memory-cached access policy with denormalized data.
        /// </summary>
        public async Task<VerifyResponse> Verify(VerifyRequest request)
        {
            try
            {
                //Validate allowed DateTime
                if (!DateTime.TryParse(request.Timestamp, null, DateTimeStyles.AdjustToUniversal, out var requestTimeUtc))
                    return Failure("Invalid timestamp format.");

                var now = DateTime.UtcNow;

                var hourDiff = Math.Abs((int)(now - requestTimeUtc).TotalHours);

                if (hourDiff > 2)
                    return Failure("Request timestamp is outside the allowed +- 2 hours window.");

                var policy = await _memory.GetAsync(request.Caller, request.Target, request.Context.ToString());

                if (policy == null || !policy.IsEnabled)
                    return Failure($"Access denied for '{request.Caller}' to '{request.Target}' ({request.Context}).");

                // Validate operation
                var operationKey = $"{request.Method} {request.Path}".ToLowerInvariant();
                var allowedOps = DelimitedStringParser.Parse(policy.Operation);

                if (!allowedOps.Any(op => operationKey.StartsWith(op)))
                {
                    return Failure($"Operation '{operationKey}' is not allowed for '{request.Caller}' -> '{request.Target}'.");
                }

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

                // Decrypt and verify signature
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
