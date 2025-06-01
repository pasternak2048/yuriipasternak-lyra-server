using LYRA.Server.Models.Verify;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities.Security;

namespace LYRA.Server.Services.SecurityVerification
{
    /// <summary>
    /// Verifies incoming signed requests by validating the digital signature using a trusted secret.
    /// This service ensures that both caller and target touchpoints are valid, active, and belong to active companies.
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

        public async Task<VerifyResponse> Verify(VerifyRequest request)
        {
            try
            {
                var builder = _factory.GetBuilder(request.Context);
                var stringToSign = builder.BuildStringToSign(request);

                var caller = await _secretProvider.GetTouchpointAsync(request.Caller);
                var target = await _secretProvider.GetTouchpointAsync(request.Target);

                var callerValidation = ValidateTouchpoint(caller, request.Caller, "caller");
                if (callerValidation != null) return callerValidation;

                var targetValidation = ValidateTouchpoint(target, request.Target, "target");
                if (targetValidation != null) return targetValidation;

                // Check if caller is authorized to access target with this context and operation
                var isAllowed = await _accessPolicyService.IsAuthorizedAsync(
                    request.Caller,
                    request.Target,
                    request.Context,
                    $"{request.Method} {request.Path}".ToLower()
                );

                if (!isAllowed)
                {
                    return new VerifyResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Access denied for '{request.Caller}' to '{request.Target}' ({request.Context})."
                    };
                }

                var encryptedSecret = caller!.UseCompanySecret
                    ? caller.Company?.Secret
                    : caller.Secret;

                if (string.IsNullOrWhiteSpace(encryptedSecret))
                {
                    var source = caller.UseCompanySecret ? "company" : "touchpoint";
                    return new VerifyResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Secret not found for caller '{request.Caller}' (source: {source})."
                    };
                }

                var secret = EncryptionHelper.DecryptSecret(encryptedSecret);
                var expectedSignature = EncryptionHelper.ComputeHmacSha512(stringToSign, secret);
                var isValid = EncryptionHelper.SecureEquals(expectedSignature, request.Signature);

                _logger.LogInformation("Verification for caller '{Caller}' and target '{Target}' completed: {Result}",
                    request.Caller, request.Target, isValid ? "SUCCESS" : "FAILURE");

                return new VerifyResponse
                {
                    IsSuccess = isValid,
                    ErrorMessage = isValid ? null : "Invalid signature."
                };
            }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning(ex, "Unsupported AccessContext '{Context}'", request.Context);
                return new VerifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Unsupported access context: {request.Context}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected verification failure");
                return new VerifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Verification failed: {ex.Message}"
                };
            }
        }

        private static VerifyResponse? ValidateTouchpoint(Entities.TrustedTouchpointEntity? tp, string systemName, string role)
        {
            if (tp == null)
                return new VerifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"{role.Capitalize()} '{systemName}' not found."
                };

            if (!tp.IsActive)
                return new VerifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"{role.Capitalize()} '{systemName}' is inactive."
                };

            if (tp.Company == null || !tp.Company.IsActive)
                return new VerifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Company of {role} '{systemName}' is inactive or missing."
                };

            return null;
        }
    }

    internal static class StringExtensions
    {
        public static string Capitalize(this string value) =>
            string.IsNullOrWhiteSpace(value) ? value : char.ToUpper(value[0]) + value[1..];
    }
}
