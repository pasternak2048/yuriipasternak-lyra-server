using LYRA.Server.Entities;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LYRA.Server.Services.AccessPolicy
{
	/// <inheritdoc/>
	public sealed class CachedAccessPolicyMemoryService : ICachedAccessPolicyMemoryService
	{
		private readonly IMemoryCache _mem;
		private readonly ICachedAccessPolicyService _persistent;
		private readonly TimeSpan _ttl;
		private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

		public CachedAccessPolicyMemoryService(
			IMemoryCache mem,
			ICachedAccessPolicyService persistent,
			IOptions<CacheOptions> options)
		{
			_mem = mem;
			_persistent = persistent;
			_ttl = options.Value.PolicyTtl;
		}

		public async Task<CachedAccessPolicyEntity?> GetAsync(string caller, string target, CancellationToken ct = default)
		{
			var key = Key(caller, target);
			if (_mem.TryGetValue(key, out CachedAccessPolicyEntity cached))
				return cached;

			var gate = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
			await gate.WaitAsync(ct);
			try
			{
				if (_mem.TryGetValue(key, out cached))
					return cached;

				var entity = await _persistent.FindAsync(Norm(caller), Norm(target), ct);
				if (entity is null) return null;

				Set(key, entity);
				return entity;
			}
			finally
			{
				gate.Release();
			}
		}

		public async Task<Dictionary<(string caller, string target), CachedAccessPolicyEntity>> GetManyAsync(
			IEnumerable<(string caller, string target)> keys, CancellationToken ct = default)
		{
			var list = keys.ToList();
			var result = new Dictionary<(string, string), CachedAccessPolicyEntity>(list.Count);
			var misses = new List<(string caller, string target)>(list.Count);

			foreach (var (c, t) in list)
			{
				var k = Key(c, t);
				if (_mem.TryGetValue(k, out CachedAccessPolicyEntity v))
					result[(Norm(c), Norm(t))] = v;
				else
					misses.Add((c, t));
			}

			if (misses.Count == 0) return result;

			var normMisses = misses
				.Select(x => (Norm(x.caller), Norm(x.target)))
				.Distinct()
				.ToList();

			var pulled = await _persistent.FindManyAsync(normMisses, ct);
			foreach (var ((nc, nt), entity) in pulled)
			{
				Set(Key(nc, nt), entity);
				result[(nc, nt)] = entity;
			}

			return result;
		}

		public async Task RefreshAsync(string caller, string target, CancellationToken ct = default)
		{
			var nc = Norm(caller);
			var nt = Norm(target);
			var entity = await _persistent.FindAsync(nc, nt, ct);
			var key = Key(nc, nt);

			if (entity is null) _mem.Remove(key);
			else Set(key, entity);
		}

		public async Task RefreshManyAsync(IEnumerable<(string caller, string target)> keys, CancellationToken ct = default)
		{
			var norm = keys.Select(k => (Norm(k.caller), Norm(k.target))).Distinct().ToList();
			if (norm.Count == 0) return;

			var pulled = await _persistent.FindManyAsync(norm, ct);

			var missing = norm.Except(pulled.Keys).ToList();

			foreach (var (c, t) in missing)
				_mem.Remove(Key(c, t));

			foreach (var ((c, t), entity) in pulled)
				Set(Key(c, t), entity);
		}

		public void Invalidate(string caller, string target)
		{
			_mem.Remove(Key(caller, target));
		}

		public void InvalidateMany(IEnumerable<(string caller, string target)> keys)
		{
			foreach (var (c, t) in keys)
				_mem.Remove(Key(c, t));
		}

		public async Task WarmupAsync(Func<CachedAccessPolicyEntity, bool>? filter = null, CancellationToken ct = default)
		{
			var all = await _persistent.GetAllAsync(ct);
			var src = filter is null ? all : all.Where(filter);
			foreach (var it in src)
				Set(Key(it.CallerSystemName, it.TargetSystemName), it);
		}

		public void Clear()
		{
			_version = Interlocked.Increment(ref _version);
		}

		private static string Norm(string s) => s?.Trim().ToLowerInvariant() ?? string.Empty;

		private int _version = 1;
		private string Key(string caller, string target)
			=> $"policy:{_version}:{Norm(caller)}→{Norm(target)}";

		private void Set(string key, CachedAccessPolicyEntity entity)
		{
			_mem.Set(key, entity, new MemoryCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = _ttl,
				Size = 1
			});
		}
	}

	public sealed class CacheOptions
	{
		public TimeSpan PolicyTtl { get; set; } = TimeSpan.FromMinutes(10);
	}
}
