using LYRA.Server.Data;
using LYRA.Server.Entities;
using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services
{
    public class AccessPolicyService : IAccessPolicyService
    {
        private readonly LyraDbContext _context;
        private readonly ILogger<AccessPolicyService> _logger;

        public AccessPolicyService(LyraDbContext context, ILogger<AccessPolicyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedResult<AccessPolicyDto>> GetPagedAsync(AccessPolicyFilters filters)
        {
            var query = _context.AccessPolicies
                .Include(p => p.Caller)
                .Include(p => p.Target)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.CallerSystemName))
            {
                query = query.Where(p =>
                    p.CallerSystemName.Contains(filters.CallerSystemName));
            }

            if (!string.IsNullOrWhiteSpace(filters.TargetSystemName))
            {
                query = query.Where(p =>
                    p.TargetSystemName.Contains(filters.TargetSystemName));
            }

            if (!string.IsNullOrWhiteSpace(filters.Operation))
            {
                query = query.Where(p => p.Operation.Contains(filters.Operation));
            }

            if (filters.Context.HasValue)
            {
                query = query.Where(p => p.Context == filters.Context.Value);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.CallerSystemName)
                .ThenBy(p => p.TargetSystemName)
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(p => new AccessPolicyDto
                {
                    Id = p.Id,
                    CallerId = p.CallerId,
                    CallerSystemName = p.CallerSystemName,
                    TargetId = p.TargetId,
                    TargetSystemName = p.TargetSystemName,
                    Operation = p.Operation,
                    Context = p.Context,
                    IsEnabled = p.IsEnabled,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return new PaginatedResult<AccessPolicyDto>
            {
                Items = items,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalItems = totalItems
            };
        }

        public async Task<AccessPolicyDto?> GetByIdAsync(Guid id)
        {
            var policy = await _context.AccessPolicies
                .Include(p => p.Caller)
                .Include(p => p.Target)
                .FirstOrDefaultAsync(p => p.Id == id);

            return policy == null ? null : MapToDto(policy);
        }

        public async Task<AccessPolicyDto> AddAsync(AccessPolicyCreateRequest request)
        {
            try
            {
                var caller = request.CallerId.HasValue
                    ? await _context.TrustedTouchpoints.FirstOrDefaultAsync(t => t.Id == request.CallerId && !t.IsDeleted)
                    : await GetTouchpointByNameAsync(request.CallerSystemName!);

                if (caller == null)
                    throw new InvalidOperationException("Caller touchpoint not found.");

                var target = request.TargetId.HasValue
                    ? await _context.TrustedTouchpoints.FirstOrDefaultAsync(t => t.Id == request.TargetId && !t.IsDeleted)
                    : await GetTouchpointByNameAsync(request.TargetSystemName!);

                if (target == null)
                    throw new InvalidOperationException("Target touchpoint not found.");

                var exists = await _context.AccessPolicies.AnyAsync(p =>
                    p.CallerSystemName == caller.SystemName &&
                    p.TargetSystemName == target.SystemName &&
                    p.Operation == request.Operation &&
                    p.Context == request.Context);

                if (exists)
                    throw new InvalidOperationException("Such policy already exists.");

                var entity = new AccessPolicyEntity
                {
                    Id = Guid.NewGuid(),
                    CallerId = caller.Id,
                    CallerSystemName = caller.SystemName,
                    TargetId = target.Id,
                    TargetSystemName = target.SystemName,
                    Operation = request.Operation.Trim(),
                    Context = request.Context,
                    IsEnabled = request.IsEnabled,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AccessPolicies.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created access policy {Caller} → {Target}: {Operation}",
                    caller.SystemName, target.SystemName, request.Operation);

                return MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create access policy.");
                throw;
            }
        }

        public async Task UpdateAsync(AccessPolicyUpdateRequest request)
        {
            try
            {
                var entity = await _context.AccessPolicies
                    .FirstOrDefaultAsync(p => p.Id == request.Id);

                if (entity is null)
                    throw new KeyNotFoundException($"Policy with ID '{request.Id}' not found.");

                // Отримуємо нових caller і target, якщо змінили systemName або ID
                TrustedTouchpointEntity? caller = null;
                TrustedTouchpointEntity? target = null;

                if (!string.IsNullOrWhiteSpace(request.CallerSystemName))
                {
                    caller = await _context.TrustedTouchpoints
                        .FirstOrDefaultAsync(t => t.SystemName == request.CallerSystemName && !t.IsDeleted);
                    if (caller == null)
                        throw new InvalidOperationException($"Caller touchpoint '{request.CallerSystemName}' not found.");
                }
                else if (request.CallerId.HasValue)
                {
                    caller = await _context.TrustedTouchpoints
                        .FirstOrDefaultAsync(t => t.Id == request.CallerId.Value && !t.IsDeleted);
                    if (caller == null)
                        throw new InvalidOperationException($"Caller touchpoint with ID '{request.CallerId}' not found.");
                }
                else
                {
                    throw new ArgumentException("CallerSystemName or CallerId must be provided.");
                }

                if (!string.IsNullOrWhiteSpace(request.TargetSystemName))
                {
                    target = await _context.TrustedTouchpoints
                        .FirstOrDefaultAsync(t => t.SystemName == request.TargetSystemName && !t.IsDeleted);
                    if (target == null)
                        throw new InvalidOperationException($"Target touchpoint '{request.TargetSystemName}' not found.");
                }
                else if (request.TargetId.HasValue)
                {
                    target = await _context.TrustedTouchpoints
                        .FirstOrDefaultAsync(t => t.Id == request.TargetId.Value && !t.IsDeleted);
                    if (target == null)
                        throw new InvalidOperationException($"Target touchpoint with ID '{request.TargetId}' not found.");
                }
                else
                {
                    throw new ArgumentException("TargetSystemName or TargetId must be provided.");
                }

                var exists = await _context.AccessPolicies.AnyAsync(p =>
                    p.Id != request.Id &&
                    p.CallerSystemName == caller.SystemName &&
                    p.TargetSystemName == target.SystemName &&
                    p.Operation == request.Operation &&
                    p.Context == request.Context);

                if (exists)
                    throw new InvalidOperationException("Such policy already exists.");

                entity.CallerId = caller.Id;
                entity.CallerSystemName = caller.SystemName;
                entity.TargetId = target.Id;
                entity.TargetSystemName = target.SystemName;
                entity.Operation = request.Operation.Trim();
                entity.Context = request.Context;
                entity.IsEnabled = request.IsEnabled;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated access policy: {PolicyId}", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update access policy.");
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var policy = await _context.AccessPolicies.FindAsync(id);
                if (policy == null)
                    return;

                _context.AccessPolicies.Remove(policy);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted access policy '{Id}'", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete access policy '{Id}'", id);
                throw;
            }
        }

        private async Task<TrustedTouchpointEntity> GetTouchpointByNameAsync(string systemName)
        {
            var entity = await _context.TrustedTouchpoints
                .FirstOrDefaultAsync(t => t.SystemName == systemName && !t.IsDeleted);

            return entity ?? throw new InvalidOperationException($"Touchpoint '{systemName}' not found.");
        }

        public async Task<int> GetTotalPolicyCountAsync()
        {
            return await _context.AccessPolicies.CountAsync();
        }

        private static AccessPolicyDto MapToDto(AccessPolicyEntity p)
        {
            return new AccessPolicyDto
            {
                Id = p.Id,
                CallerSystemName = p.CallerSystemName,
                TargetSystemName = p.TargetSystemName,
                Operation = p.Operation,
                Context = p.Context,
                IsEnabled = p.IsEnabled,
                CreatedAt = p.CreatedAt
            };
        }
    }
}
