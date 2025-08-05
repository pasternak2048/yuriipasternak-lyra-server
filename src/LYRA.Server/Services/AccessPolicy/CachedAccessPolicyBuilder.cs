using LYRA.Server.Entities;
using LYRA.Server.Services.AccessPolicy.Interfaces;

namespace LYRA.Server.Services.AccessPolicy
{
    public class CachedAccessPolicyBuilder : ICachedAccessPolicyBuilder
    {
        public CachedAccessPolicyEntity? Build(AccessPolicyEntity policy)
        {
            // Validate policy and both ends
            if (!policy.IsEnabled)
                return null;

            var caller = policy.Caller;
            var target = policy.Target;
            var callerCompany = caller.Company;
            var targetCompany = target.Company;

            if (!caller.IsActive || caller.IsDeleted)
                return null;

            if (!target.IsActive || target.IsDeleted)
                return null;

            if (!callerCompany.IsActive || callerCompany.IsDeleted)
                return null;

            if (!targetCompany.IsActive || targetCompany.IsDeleted)
                return null;

            // Determine which secret to use
            var secret = caller.UseCompanySecret
                ? callerCompany.Secret
                : caller.Secret;

            return new CachedAccessPolicyEntity
            {
                Id = policy.Id,
                CallerSystemName = policy.CallerSystemName,
                TargetSystemName = policy.TargetSystemName,
                Operation = policy.Operation.ToLowerInvariant(),
                CallerSecret = secret,
                SignatureType = caller.SignatureType.ToString(),
                IsEnabled = true,
                AllowedSourceIp = caller.AllowedSourceIp,
                CallerCompanySystemName = callerCompany.SystemName,
                TargetCompanySystemName = targetCompany.SystemName,
                CachedAtUtc = DateTime.UtcNow
            };
        }
    }
}
