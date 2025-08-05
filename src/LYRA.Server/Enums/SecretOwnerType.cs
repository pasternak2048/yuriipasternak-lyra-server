namespace LYRA.Server.Enums
{
	/// <summary>
	/// Specifies the type of entity that owns a secret key.
	/// </summary>
	public enum SecretOwnerType
	{
		/// <summary>
		/// The secret belongs to a company (shared secret).
		/// </summary>
		Company,

		/// <summary>
		/// The secret belongs to a trusted touchpoint (individual secret).
		/// </summary>
		TrustedTouchpoint
	}
}
