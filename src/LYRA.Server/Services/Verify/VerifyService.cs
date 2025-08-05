using LYRA.Security.Crypto.Core;
using LYRA.Security.Signing;
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
	/// using cached access policy data from in-memory cache for maximum performance.
	/// </summary>
	public class VerifyService : IVerifyService
	{
		private readonly ICachedAccessPolicyMemoryService _memory;
		private readonly ILogQueue _logQueue;
		private readonly ILogger<VerifyService> _loggerDotNet;

		public VerifyService(
			ICachedAccessPolicyMemoryService memory,
			ILogQueue logQueue,
			ILogger<VerifyService> loggerDotNet)
		{
			_memory = memory;
			_logQueue = logQueue;
			_loggerDotNet = loggerDotNet;
		}

		/// <summary>
		/// Performs verification using memory-cached access policy with denormalized data.
		/// Logs detailed structured entries for each failure/success scenario.
		/// </summary>
		public async Task<VerifyResponse> Verify(VerifyRequest request)
		{
			try
			{
				var meta = request.Metadata;

				// 1) Timestamp: Unix seconds -> DateTimeOffset
				if (!long.TryParse(meta.Timestamp, NumberStyles.Integer, CultureInfo.InvariantCulture, out var tsSeconds))
					return Fail("Invalid timestamp format (expected Unix seconds)", request, reason: "BadTimestamp");

				var requestTimeUtc = DateTimeOffset.FromUnixTimeSeconds(tsSeconds).UtcDateTime;
				var nowUtc = DateTime.UtcNow;
				var hourDiff = Math.Abs((nowUtc - requestTimeUtc).TotalHours);

				if (hourDiff > 2)
					return Fail("Request timestamp is outside the allowed ±2 hours window", request, reason: "Expired",
						details: $"offsetHours={hourDiff:F2}");

				// 2) Access policy lookup
				var policy = await _memory.GetAsync(meta.CallerSystemName, meta.TargetSystemName);
				if (policy == null || !policy.IsEnabled)
					return Fail("Access denied: no policy or disabled", request, reason: "PolicyDenied");

				// 3) Operation check
				var operationKey = $"{meta.Action} {meta.Resource}".ToLowerInvariant();
				var allowedOps = DelimitedStringParser.Parse(policy.Operation); // e.g. "post /api/verify"
				if (!allowedOps.Any(op => operationKey.StartsWith(op)))
					return Fail($"Operation not allowed: {operationKey}", request, reason: "OperationDenied");

				// 4) Validate payload integrity
				if (!string.IsNullOrEmpty(request.Payload))
				{
					var computedHash = EncryptionHelper.ComputeSha512(request.Payload);
					if (!string.Equals(meta.PayloadHash, computedHash, StringComparison.Ordinal))
					{
						return Fail("Payload hash mismatch — possible tampering detected", request, reason: "BadPayloadHash",
							details: $"expected={meta.PayloadHash}, actual={computedHash}");
					}
				}

				// 5) Build canonical string
				var stringToSign = SignatureStringBuilder.BuildStringToSign(meta);

				// 6) Resolve secret and verify signature
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
				_loggerDotNet.LogError(ex, "Unexpected verification failure");

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
