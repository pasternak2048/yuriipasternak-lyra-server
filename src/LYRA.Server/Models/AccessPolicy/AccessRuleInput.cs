namespace LYRA.Server.Models.AccessPolicy
{
    /// <summary>
    /// Input model for creating or editing a route rule.
    /// </summary>
    public class AccessRuleInput
    {
        /// <summary>
        /// HTTP method or ANY.
        /// ANY is normalized to "*" internally.
        /// </summary>
        public string Method { get; set; } = "GET";

        /// <summary>
        /// Path pattern.
        /// Examples: /api/orders, /api/orders/*, /*
        /// </summary>
        public string PathPattern { get; set; } = "/";
    }
}
