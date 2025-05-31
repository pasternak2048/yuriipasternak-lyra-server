namespace LYRA.Server.Enums
{
    /// <summary>
    /// Defines the type of interaction or communication context
    /// for which an access policy or signature validation applies.
    /// </summary>
    public enum AccessContext
    {
        /// <summary>
        /// HTTP request context.
        /// </summary>
        Http = 1,

        /// <summary>
        /// Event-driven context (e.g., message queues, pub/sub).
        /// </summary>
        Event = 2,

        /// <summary>
        /// Cache operation context.
        /// </summary>
        Cache = 3,

        /// <summary>
        /// gRPC communication context.
        /// </summary>
        Grpc = 4,

        /// <summary>
        /// Internal communication context within services.
        /// </summary>
        Internal = 5,

        /// <summary>
        /// SOAP web service context.
        /// </summary>
        Soap = 6
    }
}
