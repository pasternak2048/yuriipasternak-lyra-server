using LYRA.Server.Entities;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using MILANO.Client.Interfaces;
using System.Text.Json;

namespace LYRA.Server.Services.AccessPolicy
{
    /// <summary>
    /// Decorator for ICachedAccessPolicyStore that adds MILANO distributed cache
    /// support to improve performance for reads and ensure consistency for writes.
    /// </summary>
    public sealed class CachedAccessPolicyStoreDecorator : ICachedAccessPolicyStore
    {
        private readonly ICachedAccessPolicyStore _inner;
        private readonly IMilanoCacheClient _cache;
        private readonly IAccessPolicyCacheKeyBuilder _keyBuilder;
        private readonly TimeSpan _ttl;

        public CachedAccessPolicyStoreDecorator(
            ICachedAccessPolicyStore inner,
            IMilanoCacheClient cache,
            IAccessPolicyCacheKeyBuilder keyBuilder,
            TimeSpan? ttl = null)
        {
            _inner = inner;
            _cache = cache;
            _keyBuilder = keyBuilder;
            _ttl = ttl ?? TimeSpan.FromMinutes(30);
        }

        /// <inheritdoc />
        public async Task<CachedAccessPolicyEntity?> FindAsync(string caller, string target, CancellationToken ct = default)
        {
            var key = _keyBuilder.ForCallerTarget(caller, target);
            var cached = await _cache.GetAsync(key, ct);

            if (cached is not null)
                return JsonSerializer.Deserialize<CachedAccessPolicyEntity>(cached);

            var fromDb = await _inner.FindAsync(caller, target, ct);
            if (fromDb is not null)
            {
                await _cache.SetAsync(key, JsonSerializer.Serialize(fromDb), _ttl, ct);
            }

            return fromDb;
        }

        /// <inheritdoc />
        public async Task<CachedAccessPolicyEntity?> FindByIdAsync(Guid id, CancellationToken ct = default)
        {
            var key = _keyBuilder.ForId(id);
            var cached = await _cache.GetAsync(key, ct);

            if (cached is not null)
                return JsonSerializer.Deserialize<CachedAccessPolicyEntity>(cached);

            var fromDb = await _inner.FindByIdAsync(id, ct);
            if (fromDb is not null)
            {
                await _cache.SetAsync(key, JsonSerializer.Serialize(fromDb), _ttl, ct);
            }

            return fromDb;
        }

        /// <inheritdoc />
        public async Task UpsertAsync(CachedAccessPolicyEntity item, CancellationToken ct = default)
        {
            await _inner.UpsertAsync(item, ct);
            await SetMilanoKeysAsync(item, ct);
        }

        /// <inheritdoc />
        public async Task UpsertManyAsync(IEnumerable<CachedAccessPolicyEntity> items, CancellationToken ct = default)
        {
            var list = items.ToList();
            await _inner.UpsertManyAsync(list, ct);

            foreach (var item in list)
                await SetMilanoKeysAsync(item, ct);
        }

        /// <inheritdoc />
        public async Task DeleteByIdAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _inner.FindByIdAsync(id, ct);
            await _inner.DeleteByIdAsync(id, ct);

            if (entity is not null)
                await RemoveMilanoKeysAsync(entity, ct);
        }

        /// <inheritdoc />
        public async Task DeleteManyAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var list = ids.ToList();
            if (list.Count == 0) return;

            var entities = await Task.WhenAll(list.Select(id => _inner.FindByIdAsync(id, ct)));
            await _inner.DeleteManyAsync(list, ct);

            foreach (var entity in entities.Where(x => x is not null))
                await RemoveMilanoKeysAsync(entity!, ct);
        }

        /// <inheritdoc />
        public async Task<Dictionary<(string caller, string target), CachedAccessPolicyEntity>> FindManyAsync(
            IEnumerable<(string caller, string target)> keys, CancellationToken ct = default)
        {
            return await _inner.FindManyAsync(keys, ct);
        }

        /// <inheritdoc />
        public async Task ReplaceAllAsync(IEnumerable<CachedAccessPolicyEntity> items, CancellationToken ct = default)
        {
            var newItems = items.ToList();

            var oldItems = await _inner.GetAllAsync(ct);
            var newIds = newItems.Select(x => x.Id).ToHashSet();
            var newCallerTargetKeys = newItems
                .Select(x => _keyBuilder.ForCallerTarget(x.CallerSystemName, x.TargetSystemName))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            await _inner.ReplaceAllAsync(newItems, ct);

            foreach (var oldItem in oldItems)
            {
                var oldCallerTargetKey = _keyBuilder.ForCallerTarget(oldItem.CallerSystemName, oldItem.TargetSystemName);

                if (!newIds.Contains(oldItem.Id))
                    await _cache.RemoveAsync(_keyBuilder.ForId(oldItem.Id), ct);

                if (!newCallerTargetKeys.Contains(oldCallerTargetKey))
                    await _cache.RemoveAsync(oldCallerTargetKey, ct);
            }

            foreach (var item in newItems)
                await SetMilanoKeysAsync(item, ct);
        }

        /// <inheritdoc />
        public Task<List<CachedAccessPolicyEntity>> GetAllAsync(CancellationToken ct = default)
        {
            return _inner.GetAllAsync(ct);
        }

        private async Task SetMilanoKeysAsync(CachedAccessPolicyEntity item, CancellationToken ct)
        {
            var serialized = JsonSerializer.Serialize(item);

            await _cache.SetAsync(
                _keyBuilder.ForCallerTarget(item.CallerSystemName, item.TargetSystemName),
                serialized,
                _ttl,
                ct);

            await _cache.SetAsync(
                _keyBuilder.ForId(item.Id),
                serialized,
                _ttl,
                ct);
        }

        private async Task RemoveMilanoKeysAsync(CachedAccessPolicyEntity item, CancellationToken ct)
        {
            await _cache.RemoveAsync(_keyBuilder.ForId(item.Id), ct);
            await _cache.RemoveAsync(_keyBuilder.ForCallerTarget(item.CallerSystemName, item.TargetSystemName), ct);
        }
    }
}
