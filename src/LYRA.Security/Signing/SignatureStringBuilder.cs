using LYRA.Security.Internal;
using System.Text;

namespace LYRA.Security.Signing
{
	/// <summary>
	/// Builds the canonical "string to sign" from <see cref="GenericMetadata"/>.
	///
	/// Design rules:
	/// 1) Fixed key order and names (MUST NOT change):
	///    caller, target, action, resource, payloadHash, timestamp
	/// 2) Values are used as-is (raw strings, normalized).
	/// 3) The resulting string is signed and later verified byte-for-byte.
	/// </summary>
	public static class SignatureStringBuilder
	{
		/// <summary>
		/// Builds the canonical string using a fixed order and normalized raw values.
		/// Throws on null/empty mandatory fields to prevent ambiguous signatures.
		/// </summary>
		public static string BuildStringToSign(GenericMetadata m)
		{
			Guard.AgainstNull(m, nameof(m));
			Guard.AgainstNullOrEmpty(m.CallerSystemName, nameof(m.CallerSystemName));
			Guard.AgainstNullOrEmpty(m.TargetSystemName, nameof(m.TargetSystemName));
			Guard.AgainstNullOrEmpty(m.Action, nameof(m.Action));
			Guard.AgainstNullOrEmpty(m.Resource, nameof(m.Resource));
			Guard.AgainstNullOrEmpty(m.PayloadHash, nameof(m.PayloadHash));
			Guard.AgainstNullOrEmpty(m.Timestamp, nameof(m.Timestamp));

			// Normalize all fields (e.g., Unicode Form C) to ensure deterministic behavior.
			var caller = Normalize(m.CallerSystemName);
			var target = Normalize(m.TargetSystemName);
			var action = Normalize(m.Action);
			var resource = Normalize(m.Resource);
			var payload = Normalize(m.PayloadHash);
			var timestamp = Normalize(m.Timestamp);

			return string.Join("&", new[]
			{
			"caller="      + caller,
			"target="      + target,
			"action="      + action,
			"resource="    + resource,
			"payloadHash=" + payload,
			"timestamp="   + timestamp
		});
		}

		/// <summary>
		/// Normalizes a string using Unicode Form C to ensure consistency across platforms.
		/// </summary>
		private static string Normalize(string value)
		{
			if (value is null) throw new ArgumentNullException(nameof(value));
			return value.Normalize(NormalizationForm.FormC);
		}
	}
}
