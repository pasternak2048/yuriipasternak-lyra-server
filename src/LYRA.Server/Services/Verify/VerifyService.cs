using LYRA.Security.Crypto.Core;
using LYRA.Security.Signing;
using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.Logging;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Services.Logging.Interfaces;
using LYRA.Server.Services.Verify.Interfaces;
using LYRA.Server.Utilities;
using LYRA.Server.Utilities.Security;
using System.Globalization;

namespace LYRA.Server.Services.Verify
{
    /// <summary>
    /// Service responsible for verifying signed requests by validating the digital signature
    /// using access policy data retrieved via distributed + memory cache for performance and consistency.
    /// </summary>
    public class VerifyService : IVerifyService
    {
        private readonly ICachedAccessPolicyStore _policyStore;
        private readonly ILogQueue _logQueue;
        private readonly ILogger<VerifyService> _logger;

        public VerifyService(
            ICachedAccessPolicyStore policyStore,
            ILogQueue logQueue,
            ILogger<VerifyService> logger)
        {
            _policyStore = policyStore;
            _logQueue = logQueue;
            _logger = logger;
        }

        /// <summary>
        /// Performs verification using cached access policies (MILANO or in-memory).
        /// Logs detailed structured entries for each failure/success scenario.
        /// </summary>
        public async Task<VerifyResponse> Verify(VerifyRequest request)
        {
            try
            {
                var meta = request.Metadata;

                // 1) Timestamp
                if (!long.TryParse(meta.Timestamp, NumberStyles.Integer, CultureInfo.InvariantCulture, out var tsSeconds))
                    return Fail("Invalid timestamp format (expected Unix seconds)", request, reason: "BadTimestamp");

                var requestTimeUtc = DateTimeOffset.FromUnixTimeSeconds(tsSeconds).UtcDateTime;
                var nowUtc = DateTime.UtcNow;
                var hourDiff = Math.Abs((nowUtc - requestTimeUtc).TotalHours);

                if (hourDiff > 2)
                    return Fail("Request timestamp is outside the allowed ±2 hours window", request, reason: "Expired",
                        details: $"offsetHours={hourDiff:F2}");

                // 2) Access policy lookup (Distributed cache + fallback)
                var policy = await _policyStore.FindAsync(meta.CallerSystemName, meta.TargetSystemName);
                if (policy is null || !policy.IsEnabled)
                    return Fail("Access denied: no policy or disabled", request, reason: "PolicyDenied");

                // 3) Method + path check using current Operation storage
                var requestedMethod = OperationParser.NormalizeMethod(meta.Method);
                var requestedPath = OperationParser.NormalizePath(meta.Path);

                var rules = AccessRuleParser.Parse(policy.Operation);

                var isAllowed = rules.Any(rule =>
                    string.Equals(
                        requestedMethod,
                        rule.Method,
                        StringComparison.OrdinalIgnoreCase)
                    && OperationParser.PathMatches(requestedPath, rule.PathPattern)
                );

                if (!isAllowed)
                    return Fail(
                        $"Operation not allowed: {requestedMethod} {requestedPath}",
                        request,
                        reason: "OperationDenied");

                // 4) Body hash
                if (!string.IsNullOrEmpty(request.Payload))
                {
                    var computedHash = EncryptionHelper.ComputeSha512(request.Payload);
                    if (!string.Equals(meta.BodyHash, computedHash, StringComparison.Ordinal))
                    {
                        return Fail("Payload hash mismatch — possible tampering detected", request, reason: "BadPayloadHash",
                            details: $"expected={meta.BodyHash}, actual={computedHash}");
                    }
                }

                // 5) Canonical string
                var stringToSign = SignatureStringBuilder.BuildStringToSign(meta);

                // 6) Decrypt secret & validate signature
                var decryptedSecret = EncryptionHelper.DecryptSecret(policy.CallerSecret);
                var ok = Signer.Verify(stringToSign, decryptedSecret, request.Signed.Signature, request.Signed.SignatureType);

                if (!ok)
                    return Fail("Invalid signature", request, reason: "BadSignature", signatureHash: request.Signed.Signature);

                // 7) Success
                _logQueue.Enqueue(new LogEntryDto
                {
                    Type = "Verification",
                    Status = "Success",
                    Description = "Request verified successfully",
                    CallerSystem = meta.CallerSystemName,
                    TargetSystem = meta.TargetSystemName,
                    SignatureHash = request.Signed.Signature,
                    Source = nameof(VerifyService)
                });

                return new VerifyResponse { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected verification failure");

                return Fail("Exception during verification", request,
                    reason: "Error", exception: ex.ToString(), signatureHash: request.Signed?.Signature);
            }
        }

        private VerifyResponse Fail(
            string description,
            VerifyRequest request,
            string reason,
            string? exception = null,
            string? signatureHash = null,
            string? details = null,
            string status = "Fail")
        {
            var meta = request.Metadata;

            _logQueue.Enqueue(new LogEntryDto
            {
                Type = "Verification",
                Status = status,
                Description = description,
                CallerSystem = meta.CallerSystemName,
                TargetSystem = meta.TargetSystemName,
                Exception = exception,
                SignatureHash = signatureHash,
                Source = nameof(VerifyService)
            });

            return new VerifyResponse
            {
                Success = false,
                Reason = reason,
                Details = details ?? description
            };
        }
    }
}
