using LYRA.Server.Entities;
using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using System.Text.Json;

namespace LYRA.Server.Services.AccessPolicy
{
    public class CachedAccessPolicyBuilder : ICachedAccessPolicyBuilder
    {
        public CachedAccessPolicyEntity? Build(AccessPolicyEntity policy)
        {
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

            var secret = caller.UseCompanySecret
                ? callerCompany.Secret
                : caller.Secret;

            var rules = policy.Rules
                .OrderBy(r => r.HttpMethod)
                .ThenBy(r => r.PathPattern)
                .Select(r => new AccessRule
                {
                    Method = r.HttpMethod,
                    PathPattern = r.PathPattern
                })
                .ToList();

            return new CachedAccessPolicyEntity
            {
                Id = policy.Id,
                CallerSystemName = policy.CallerSystemName,
                TargetSystemName = policy.TargetSystemName,
                RulesJson = JsonSerializer.Serialize(rules),
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
