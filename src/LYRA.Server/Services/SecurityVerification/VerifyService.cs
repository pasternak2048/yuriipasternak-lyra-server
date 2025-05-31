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

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyService"/> class.
        /// </summary>
        /// <param name="factory">Factory used to construct canonical strings for signature verification.</param>
        /// <param name="secretProvider">Provider used to retrieve metadata and secrets for trusted touchpoints.</param>
        public VerifyService(SignatureStringBuilderFactory factory, ISecretProvider secretProvider)
        {
            _factory = factory;
            _secretProvider = secretProvider;
        }

        /// <summary>
        /// Verifies a request by checking its digital signature against the expected value using HMAC SHA512.
        /// Ensures the caller and target are valid and active, and verifies the integrity of the signed data.
        /// </summary>
        /// <param name="request">The request containing caller, target, payload hash, and signature data.</param>
        /// <returns>
        /// A <see cref="VerifyResponse"/> indicating whether the request is valid and, if not, describing the reason for failure.
        /// </returns>
        public async Task<VerifyResponse> Verify(VerifyRequest request)
        {
            try
            {
                var builder = _factory.GetBuilder(request.Context);
                var stringToSign = builder.BuildStringToSign(request);

                var callerTouchpoint = await _secretProvider.GetTouchpointAsync(request.Caller);
                if (callerTouchpoint == null)
                {
                    return new VerifyResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Caller '{request.Caller}' not found."
                    };
                }

                if (!callerTouchpoint.IsActive)
                {
                    return new VerifyResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Caller '{request.Caller}' is inactive."
                    };
                }

                if (callerTouchpoint.Company == null || !callerTouchpoint.Company.IsActive)
                {
                    return new VerifyResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Company of caller '{request.Caller}' is inactive or missing."
                    };
                }

                var targetTouchpoint = await _secretProvider.GetTouchpointAsync(request.Target);
                if (targetTouchpoint == null)
                {
                    return new VerifyResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Target '{request.Target}' not found."
                    };
                }

                if (!targetTouchpoint.IsActive)
                {
                    return new VerifyResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Target '{request.Target}' is inactive."
                    };
                }

                if (targetTouchpoint.Company == null || !targetTouchpoint.Company.IsActive)
                {
                    return new VerifyResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Company of target '{request.Target}' is inactive or missing."
                    };
                }

                var encryptedSecret = callerTouchpoint.UseCompanySecret
                    ? callerTouchpoint.Company.Secret
                    : callerTouchpoint.Secret;

                if (string.IsNullOrEmpty(encryptedSecret))
                {
                    return new VerifyResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Secret not found for caller '{request.Caller}'."
                    };
                }

                var secret = EncryptionHelper.DecryptSecret(encryptedSecret);

                var expectedSignature = EncryptionHelper.ComputeHmacSha512(stringToSign, secret);

                bool isValid = EncryptionHelper.SecureEquals(expectedSignature, request.Signature);

                return new VerifyResponse
                {
                    IsSuccess = isValid,
                    ErrorMessage = isValid ? null : "Invalid signature."
                };
            }
            catch (Exception ex)
            {
                return new VerifyResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Verification failed: {ex.Message}"
                };
            }
        }
    }
}
