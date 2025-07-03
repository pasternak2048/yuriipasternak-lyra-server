using LYRA.Security.Models.Verify;
using LYRA.Security.Signature;
using LYRA.Security.Utilities.Security;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.Logging.Interfaces;
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
        private readonly ILogService _logger;
        private readonly ILogger<VerifyService> _loggerDotNet;

        public VerifyService(
            SignatureStringBuilderFactory factory,
            ICachedAccessPolicyMemoryService memory,
            ILogService logger,
            ILogger<VerifyService> loggerDotNet)
        {
            _factory = factory;
            _memory = memory;
            _logger = logger;
            _loggerDotNet = loggerDotNet;
        }

        /// <summary>
        /// Performs verification using memory-cached access policy with denormalized data.
        /// Logs detailed structured entries for each failure/success scenario.
        /// </summary>
        /// <param name="request">Request to verify.</param>
        /// <returns>Verification result.</returns>
        public async Task<VerifyResponse> Verify(VerifyRequest request)
        {
            try
            {
                // Parse and validate the timestamp.
                if (!DateTime.TryParse(request.Timestamp, null, DateTimeStyles.AdjustToUniversal, out var requestTimeUtc))
                {
                    return await FailAsync(
                        description: "Invalid timestamp format",
                        request: request);
                }

                // Check that the timestamp is within the acceptable range (±2 hours).
                var now = DateTime.UtcNow;
                var hourDiff = Math.Abs((int)(now - requestTimeUtc).TotalHours);
                if (hourDiff > 2)
                {
                    return await FailAsync(
                        description: "Request timestamp is outside the allowed +- 2 hours window.",
                        request: request);
                }

                // Retrieve cached policy for caller-target-context combination.
                var policy = await _memory.GetAsync(request.Caller, request.Target, request.Context.ToString());
                if (policy == null || !policy.IsEnabled)
                {
                    return await FailAsync(
                        description: "Access denied: no policy or disabled",
                        request: request);
                }

                // Validate the requested operation against allowed operations in policy.
                var operationKey = $"{request.Method} {request.Path}".ToLowerInvariant();
                var allowedOps = DelimitedStringParser.Parse(policy.Operation);
                if (!allowedOps.Any(op => operationKey.StartsWith(op)))
                {
                    return await FailAsync(
                        description: $"Operation not allowed: {operationKey}",
                        request: request);
                }

                // Check if payload validation is required for the given context and method.
                var metadata = SignatureContextRegistry.GetMetadata(request.Context);
                if (metadata.RequiresPayloadHash(request))
                {
                    if (string.IsNullOrWhiteSpace(request.Payload))
                    {
                        return await FailAsync(
                            description: "Payload is required for this context and method.",
                            request: request);
                    }

                    if (string.IsNullOrWhiteSpace(request.PayloadHash))
                    {
                        return await FailAsync(
                            description: "PayloadHash is required for this context and method.",
                            request: request);
                    }

                    var computed = EncryptionHelper.ComputeSha512(request.Payload);
                    if (!EncryptionHelper.SecureEquals(computed, request.PayloadHash))
                    {
                        return await FailAsync(
                            description: "PayloadHash does not match payload.",
                            request: request);
                    }
                }

                // Construct the string to sign using the selected builder.
                var builder = _factory.GetBuilder(request.Context);
                var stringToSign = builder.BuildStringToSign(
                    request.Caller, request.Target, request.Method.ToLowerInvariant(), request.Path.ToLowerInvariant(),
                    request.PayloadHash, request.Timestamp);

                // Compute HMAC and compare to signature provided in request.
                var decryptedSecret = EncryptionHelper.DecryptSecret(policy.CallerSecret);
                var expectedSignature = EncryptionHelper.ComputeHmacSha512(stringToSign, decryptedSecret);
                var isValid = EncryptionHelper.SecureEquals(expectedSignature, request.Signature);

                if (!isValid)
                {
                    return await FailAsync(
                        description: "Invalid signature.",
                        request: request,
                        signatureHash: request.Signature);
                }

                // Success log
                await _logger.WriteAsync(
                    type: "Verification",
                    status: "Success",
                    description: "Request verified successfully",
                    callerSystem: request.Caller,
                    targetSystem: request.Target,
                    signatureHash: request.Signature,
                    source: nameof(VerifyService));

                return VerifyResponse.Success;
            }
            catch (Exception ex)
            {
                // Unexpected error log
                _loggerDotNet.LogError(ex, "Unexpected verification failure");

                return await FailAsync(
                    description: "Exception during verification",
                    request: request,
                    exception: ex.ToString(),
                    signatureHash: request.Signature,
                    status: "Error");
            }
        }

        /// <summary>
        /// Logs a failure message and returns a failed <see cref="VerifyResponse"/>.
        /// </summary>
        /// <param name="description">Short description of the failure reason (e.g., "Invalid signature").</param>
        /// <param name="request">Original verification request for logging context.</param>
        /// <param name="exception">Optional exception message (stack trace or message).</param>
        /// <param name="signatureHash">Optional signature involved in the failure.</param>
        /// <param name="status">Log severity (Fail, Error, Warning, etc.).</param>
        /// <returns>A failed <see cref="VerifyResponse"/> with the specified message.</returns>
        private async Task<VerifyResponse> FailAsync(
            string description,
            VerifyRequest request,
            string? exception = null,
            string? signatureHash = null,
            string status = "Fail")
        {
            await _logger.WriteAsync(
                type: "Verification",
                status: status,
                description: description,
                callerSystem: request.Caller,
                targetSystem: request.Target,
                exception: exception,
                signatureHash: signatureHash,
                source: nameof(VerifyService));

            return new VerifyResponse
            {
                IsSuccess = false,
                ErrorMessage = description
            };
        }
    }
}
