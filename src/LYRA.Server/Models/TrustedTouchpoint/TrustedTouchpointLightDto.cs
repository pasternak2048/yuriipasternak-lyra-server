namespace LYRA.Server.Models.TrustedTouchpoint
{
    /// <summary>
    /// Lightweight DTO representing a minimal view of a trusted touchpoint,
    /// typically used in dropdowns, lookups, or lists where full detail is unnecessary.
    /// </summary>
    public class TrustedTouchpointLightDto
    {
        /// <summary>
        /// Unique identifier of the touchpoint.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Machine-readable system name (used as a unique reference key).
        /// </summary>
        public string SystemName { get; set; } = null!;

        /// <summary>
        /// Human-readable name for display purposes (e.g. in UI).
        /// </summary>
        public string DisplayName { get; set; } = null!;
    }
}
