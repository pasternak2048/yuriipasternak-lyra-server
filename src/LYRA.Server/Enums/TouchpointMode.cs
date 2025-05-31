namespace LYRA.Server.Enums
{
    /// <summary>
    /// Defines the communication role of a trusted touchpoint.
    /// </summary>
    public enum TouchpointMode
    {
        /// <summary>
        /// Touchpoint acts only as a caller (initiates requests).
        /// </summary>
        CallerOnly,

        /// <summary>
        /// Touchpoint acts only as a target (receives requests).
        /// </summary>
        TargetOnly,

        /// <summary>
        /// Touchpoint can act as both caller and target.
        /// </summary>
        Both
    }
}
