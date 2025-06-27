using LYRA.Server.Entities;

namespace LYRA.Server.Services.Interfaces
{
    public interface ICachedAccessPolicyBuilder
    {
        /// <summary>
        /// Builds a cached policy from a fully-loaded AccessPolicyEntity.
        /// Returns null if the policy or related entities are disabled or deleted.
        /// </summary>
        CachedAccessPolicyEntity? Build(AccessPolicyEntity policy);
    }
}
