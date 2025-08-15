using LYRA.Security.Enums;
using LYRA.Server.Data.LyraDb;
using LYRA.Server.Entities;
using LYRA.Server.Models.AccessPolicy;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Services.AccessPolicy.Interfaces;
using LYRA.Server.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services.AccessPolicy
{
    /// <summary>
    /// Service responsible for managing access policies that define allowed operations 
    /// between trusted touchpoints across different access contexts.
    /// </summary>
    public class AccessPolicyService : IAccessPolicyService
    {
        private readonly LyraDbContext _context;
        private readonly ILogger<AccessPolicyService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessPolicyService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger for audit and diagnostics.</param>
        public AccessPolicyService(LyraDbContext context, ILogger<AccessPolicyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<PaginatedResult<AccessPolicyDto>> GetPagedAsync(AccessPolicyFilters filters)
        {
            var query = _context.AccessPolicies
                .AsNoTracking().AsQueryable();

            if (filters.CallerId.HasValue)
                query = query.Where(p => p.CallerId == filters.CallerId.Value);

            if (filters.TargetId.HasValue)
                query = query.Where(p => p.TargetId == filters.TargetId.Value);

            if (!string.IsNullOrWhiteSpace(filters.Operation))
                query = query.Where(p => p.Operation.Contains(filters.Operation));

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

        /// <inheritdoc />
        public async Task<AccessPolicyDto?> GetByIdAsync(Guid id)
        {
            var policy = await _context.AccessPolicies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return policy == null ? null : MapToDto(policy);
        }

        /// <inheritdoc />
        public async Task<AccessPolicyDto> AddAsync(AccessPolicyCreateRequest request)
        {
            try
            {
                var caller = await ResolveTouchpointAsync(request.CallerId, request.CallerSystemName, "Caller");
                var target = await ResolveTouchpointAsync(request.TargetId, request.TargetSystemName, "Target");

                if (await PolicyExists(null, caller.SystemName, target.SystemName))
                    throw new InvalidOperationException("Such policy already exists.");

                var entity = new AccessPolicyEntity
                {
                    Id = Guid.NewGuid(),
                    CallerId = caller.Id,
                    CallerSystemName = caller.SystemName,
                    TargetId = target.Id,
                    TargetSystemName = target.SystemName,
                    Operation = DelimitedStringParser.Join(request.Operations),
                    IsEnabled = request.IsEnabled,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AccessPolicies.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created access policy {Caller} → {Target}: {Operation}",
                    caller.SystemName, target.SystemName, request.Operations);

                return MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create access policy.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(AccessPolicyUpdateRequest request)
        {
            try
            {
                var entity = await _context.AccessPolicies.FirstOrDefaultAsync(p => p.Id == request.Id);
                if (entity == null)
                    throw new KeyNotFoundException($"Policy with ID '{request.Id}' not found.");

                var caller = await ResolveTouchpointAsync(request.CallerId, request.CallerSystemName, "Caller");
                var target = await ResolveTouchpointAsync(request.TargetId, request.TargetSystemName, "Target");

                if (await PolicyExists(request.Id, caller.SystemName, target.SystemName))
                    throw new InvalidOperationException("Such policy already exists.");

                entity.CallerId = caller.Id;
                entity.CallerSystemName = caller.SystemName;
                entity.TargetId = target.Id;
                entity.TargetSystemName = target.SystemName;
                entity.Operation = DelimitedStringParser.Join(request.Operations);
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

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var policy = await _context.AccessPolicies.FindAsync(id);
                if (policy == null) return;

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

        /// <inheritdoc />
        public async Task<bool> IsAuthorizedAsync(string caller, string target, string operation)
        {
            var normalizedOperation = operation.ToLowerInvariant().Trim();

            return await _context.AccessPolicies.AsNoTracking().AnyAsync(p =>
                p.CallerSystemName == caller &&
                p.TargetSystemName == target &&
                p.Operation == normalizedOperation &&
                p.IsEnabled);
        }

        /// <inheritdoc />
        public async Task<int> GetTotalPolicyCountAsync()
        {
            return await _context.AccessPolicies.AsNoTracking().CountAsync();
        }

        private static AccessPolicyDto MapToDto(AccessPolicyEntity p) => new()
        {
            Id = p.Id,
            CallerSystemName = p.CallerSystemName,
            TargetSystemName = p.TargetSystemName,
            CallerId = p.CallerId,
            TargetId = p.TargetId,
            Operation = p.Operation,
            IsEnabled = p.IsEnabled,
            CreatedAt = p.CreatedAt
        };

        /// <summary>
        /// Returns a base query for only active (not deleted) touchpoints.
        /// </summary>
        private IQueryable<TrustedTouchpointEntity> ActiveTouchpoints()
        {
            return _context.TrustedTouchpoints.AsNoTracking().Where(t => !t.IsDeleted);
        }

        /// <summary>
        /// Returns whether a policy already exists for given parameters (excluding a specific ID).
        /// </summary>
        private async Task<bool> PolicyExists(Guid? policyId, string caller, string target)
        {
            return await _context.AccessPolicies.AsNoTracking().AnyAsync(p =>
                (!policyId.HasValue || p.Id != policyId.Value) &&
                p.CallerSystemName == caller &&
                p.TargetSystemName == target);
        }

        private async Task<TrustedTouchpointEntity> GetTouchpointByNameAsync(string systemName)
        {
            var entity = await ActiveTouchpoints()
                .FirstOrDefaultAsync(t => t.SystemName == systemName);

            return entity ?? throw new InvalidOperationException($"Touchpoint '{systemName}' not found.");
        }

        private async Task<TrustedTouchpointEntity> ResolveTouchpointAsync(Guid? id, string? systemName, string role)
        {
            TrustedTouchpointEntity? touchpoint = null;

            if (id.HasValue)
                touchpoint = await ActiveTouchpoints().FirstOrDefaultAsync(t => t.Id == id.Value);
            else if (!string.IsNullOrWhiteSpace(systemName))
                touchpoint = await GetTouchpointByNameAsync(systemName);

            if (touchpoint == null)
                throw new InvalidOperationException($"{role} touchpoint not found.");

            return touchpoint;
        }
    }
}
