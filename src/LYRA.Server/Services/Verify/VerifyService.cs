using LYRA.Server.Models.Verify;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities.Security;

namespace LYRA.Server.Services.Verify
{
    public class VerifyService : IVerifyService
    {
        private readonly SignatureStringBuilderFactory _factory;
        private readonly ISecretProvider _secretProvider;

        public VerifyService(SignatureStringBuilderFactory factory, ISecretProvider secretProvider)
        {
            _factory = factory;
            _secretProvider = secretProvider;
        }

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
