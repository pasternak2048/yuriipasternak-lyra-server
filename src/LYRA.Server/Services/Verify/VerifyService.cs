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
using System.Text.Json;

namespace LYRA.Server.Services.Verify
{
    /// <summary>
    /// Service responsible for verifying signed requests by validating the digital signature
    /// using access policy data retrieved via distributed + memory cache for performance and consistency.
    /// </summary>
    public class VerifyService : IVerifyService
    {
        private static readonly TimeSpan AllowedTimestampSkew = TimeSpan.FromHours(2);

        private readonly ICachedAccessPolicyStore _policyStore;
        private readonly IReplayProtectionStore _replayProtectionStore;
        private readonly ILogQueue _logQueue;
        private readonly ILogger<VerifyService> _logger;

        public VerifyService(
            ICachedAccessPolicyStore policyStore,
            IReplayProtectionStore replayProtectionStore,
            ILogQueue logQueue,
            ILogger<VerifyService> logger)
        {
            _policyStore = policyStore;
            _replayProtectionStore = replayProtectionStore;
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

                if (string.IsNullOrWhiteSpace(request.RequestId))
                    return Fail("Missing request ID", request, reason: "MissingRequestId");

                if (!long.TryParse(meta.Timestamp, NumberStyles.Integer, CultureInfo.InvariantCulture, out var tsSeconds))
                    return Fail("Invalid timestamp format (expected Unix seconds)", request, reason: "BadTimestamp");

                var requestTimeUtc = DateTimeOffset.FromUnixTimeSeconds(tsSeconds).UtcDateTime;
                var nowUtc = DateTime.UtcNow;
                var hourDiff = Math.Abs((nowUtc - requestTimeUtc).TotalHours);

                if (hourDiff > AllowedTimestampSkew.TotalHours)
                    return Fail("Request timestamp is outside the allowed ±2 hours window", request, reason: "Expired",
                        details: $"offsetHours={hourDiff:F2}");

                var replayTtl = CalculateReplayTtl(requestTimeUtc, nowUtc);

                var requestIdAccepted = await _replayProtectionStore.TryMarkAsUsedAsync(
                    meta.CallerSystemName,
                    meta.TargetSystemName,
                    request.RequestId,
                    replayTtl);

                if (!requestIdAccepted)
                    return Fail("Replay detected: request ID was already used", request, reason: "ReplayDetected");

                var policy = await _policyStore.FindAsync(meta.CallerSystemName, meta.TargetSystemName);
                if (policy is null || !policy.IsEnabled)
                    return Fail("Access denied: no policy or disabled", request, reason: "PolicyDenied");

                var requestedMethod = RouteRuleMatcher.NormalizeMethod(meta.Method);
                var requestedPath = RouteRuleMatcher.NormalizePath(meta.Path);

                var rules = JsonSerializer.Deserialize<List<AccessRule>>(policy.RulesJson) ?? new List<AccessRule>();

                var isAllowed = rules.Any(rule =>
                    RouteRuleMatcher.MethodMatches(requestedMethod, rule.Method) &&
                    RouteRuleMatcher.PathMatches(requestedPath, rule.PathPattern));

                if (!isAllowed)
                    return Fail(
                        $"Operation not allowed: {requestedMethod} {requestedPath}",
                        request,
                        reason: "OperationDenied");

                var payload = request.Payload ?? string.Empty;
                var computedHash = EncryptionHelper.ComputeSha512(payload);

                if (!string.Equals(meta.BodyHash, computedHash, StringComparison.Ordinal))
                {
                    return Fail("Payload hash mismatch — possible tampering detected", request, reason: "BadPayloadHash",
                        details: $"expected={meta.BodyHash}, actual={computedHash}");
                }

                var stringToSign = SignatureStringBuilder.BuildStringToSign(meta);

                var decryptedSecret = EncryptionHelper.DecryptSecret(policy.CallerSecret);
                var ok = Signer.Verify(stringToSign, decryptedSecret, request.Signed.Signature, request.Signed.SignatureType);

                if (!ok)
                    return Fail("Invalid signature", request, reason: "BadSignature", signatureHash: request.Signed.Signature);

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

        private static TimeSpan CalculateReplayTtl(DateTime requestTimeUtc, DateTime nowUtc)
        {
            var expiresAtUtc = requestTimeUtc.Add(AllowedTimestampSkew);

            if (expiresAtUtc <= nowUtc)
                return TimeSpan.FromMinutes(1);

            return expiresAtUtc - nowUtc;
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
