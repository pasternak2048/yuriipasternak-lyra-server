using LYRA.Server.Data.LyraCachedDb;
using LYRA.Server.Entities;
using LYRA.Server.Services.Interfaces;

namespace LYRA.Server.Services
{
    /// <summary>
    /// Service for managing cached access policies in the LyraCachedDbContext.
    /// Used to optimize runtime verification by storing denormalized access data.
    /// </summary>
    public class CachedAccessPolicyService : ICachedAccessPolicyService
    {
        private readonly LyraCachedDbContext _db;

        public CachedAccessPolicyService(LyraCachedDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc/>
        public async Task ReplaceAllAsync(IEnumerable<CachedAccessPolicyEntity> items)
        {
            _db.CachedAccessPolicies.RemoveRange(_db.CachedAccessPolicies);
            await _db.SaveChangesAsync();

            await _db.CachedAccessPolicies.AddRangeAsync(items);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpsertAsync(CachedAccessPolicyEntity item)
        {
            var existing = await _db.CachedAccessPolicies.FindAsync(item.Id);
            if (existing != null)
            {
                _db.Entry(existing).CurrentValues.SetValues(item);
            }
            else
            {
                await _db.CachedAccessPolicies.AddAsync(item);
            }

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteByIdAsync(Guid id)
        {
            var entity = await _db.CachedAccessPolicies.FindAsync(id);
            if (entity != null)
            {
                _db.CachedAccessPolicies.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task ClearAsync()
        {
            _db.CachedAccessPolicies.RemoveRange(_db.CachedAccessPolicies);
            await _db.SaveChangesAsync();
        }
    }
}
