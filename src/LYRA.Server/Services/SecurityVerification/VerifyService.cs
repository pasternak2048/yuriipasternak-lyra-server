using LYRA.Security.Models.Verify;
using LYRA.Security.Signature;
using LYRA.Security.Utilities.Security;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;

namespace LYRA.Server.Services.SecurityVerification
{
    /// <summary>
    /// Service responsible for verifying signed requests by validating the digital signature
    /// and confirming authorization between trusted touchpoints.
    /// </summary>
    public class VerifyService : IVerifyService
    {
        private readonly SignatureStringBuilderFactory _factory;
        private readonly ISecretProvider _secretProvider;
        private readonly ILogger<VerifyService> _logger;
        private readonly IAccessPolicyService _accessPolicyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyService"/> class.
        /// </summary>
        public VerifyService(
            SignatureStringBuilderFactory factory,
            ISecretProvider secretProvider,
            ILogger<VerifyService> logger,
            IAccessPolicyService accessPolicyService)
        {
            _factory = factory;
            _secretProvider = secretProvider;
            _logger = logger;
            _accessPolicyService = accessPolicyService;
        }

        /// <summary>
        /// Performs the full verification flow for a signed request:
        /// string-to-sign generation, touchpoint validation, policy check, and signature match.
        /// </summary>
        public async Task<VerifyResponse> Verify(VerifyRequest request)
        {
            try
            {
                var builder = _factory.GetBuilder(request.Context);
                var stringToSign = builder.BuildStringToSign(
                    caller: request.Caller,
                    target: request.Target,
                    method: request.Method,
                    path: request.Path,
                    payloadHash: request.PayloadHash,
                    timestamp: request.Timestamp
                );

                // Retrieve lightweight info from provider
                var caller = await _secretProvider.GetTouchpointAsync(request.Caller);
                var target = await _secretProvider.GetTouchpointAsync(request.Target);

                // Validate caller and target
                var callerValidation = ValidateTouchpoint(caller, request.Caller, "caller");
                if (callerValidation != null) return callerValidation;

                var targetValidation = ValidateTouchpoint(target, request.Target, "target");
                if (targetValidation != null) return targetValidation;

                // Verify access policy
                var operationKey = $"{request.Method} {request.Path}".ToLowerInvariant();
                var authorized = await _accessPolicyService.IsAuthorizedAsync(
                    request.Caller, request.Target, request.Context, operationKey);

                if (!authorized)
                {
                    return Failure($"Access denied for '{request.Caller}' to '{request.Target}' ({request.Context}).");
                }

                // Verify payload hash
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

                // Determine source of secret
                var encryptedSecret = caller!.UseCompanySecret
                    ? caller.CompanySecret
                    : caller.Secret;

                if (string.IsNullOrWhiteSpace(encryptedSecret))
                {
                    var source = caller.UseCompanySecret ? "company" : "touchpoint";
                    return Failure($"Secret not found for caller '{request.Caller}' (source: {source}).");
                }

                var secret = EncryptionHelper.DecryptSecret(encryptedSecret);
                var expectedSignature = EncryptionHelper.ComputeHmacSha512(stringToSign, secret);
                var isValid = EncryptionHelper.SecureEquals(expectedSignature, request.Signature);

                _logger.LogInformation("Verification result for {Caller} → {Target}: {Result}",
                    request.Caller, request.Target, isValid ? "SUCCESS" : "FAILURE");

                return isValid ? VerifyResponse.Success : Failure("Invalid signature.");
            }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning(ex, "Unsupported AccessContext '{Context}'", request.Context);
                return Failure($"Unsupported access context: {request.Context}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected verification failure");
                return Failure($"Verification failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates a simplified touchpoint info object and its company status.
        /// </summary>
        private static VerifyResponse? ValidateTouchpoint(TrustedTouchpointInfo? tp, string systemName, string role)
        {
            if (tp is null)
                return Failure($"{role.Capitalize()} '{systemName}' not found.");

            if (!tp.IsActive)
                return Failure($"{role.Capitalize()} '{systemName}' is inactive.");

            if (!tp.IsCompanyActive)
                return Failure($"Company of {role} '{systemName}' is inactive.");

            return null;
        }

        /// <summary>
        /// Creates a failed response with the provided error message.
        /// </summary>
        private static VerifyResponse Failure(string message) => new()
        {
            IsSuccess = false,
            ErrorMessage = message
        };
    }

    /// <summary>
    /// String extension for capitalizing the first character.
    /// </summary>
    internal static class StringExtensions
    {
        public static string Capitalize(this string value) =>
            string.IsNullOrWhiteSpace(value) ? value : char.ToUpperInvariant(value[0]) + value[1..];
    }
}
