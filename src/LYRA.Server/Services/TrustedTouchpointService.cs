using LYRA.Security.Enums;
using LYRA.Security.Utilities;
using LYRA.Security.Utilities.Security;
using LYRA.Server.Data;
using LYRA.Server.Entities;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.Shared;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services
{
    /// <summary>
    /// Service responsible for managing trusted touchpoints (agents) that initiate or receive requests
    /// in the inter-service communication network.
    /// </summary>
    public class TrustedTouchpointService : ITrustedTouchpointService
    {
        private readonly LyraDbContext _context;
        private readonly ILogger<TrustedTouchpointService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrustedTouchpointService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">Logger for audit and diagnostics.</param>
        public TrustedTouchpointService(LyraDbContext context, ILogger<TrustedTouchpointService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<List<TrustedTouchpointLightDto>> GetLightweightAsync()
        {
            return await _context.TrustedTouchpoints
                .Where(t => t.IsActive && !t.IsDeleted)
                .OrderBy(t => t.SystemName)
                .Select(t => new TrustedTouchpointLightDto
                {
                    Id = t.Id,
                    SystemName = t.SystemName,
                    DisplayName = t.DisplayName
                })
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<PaginatedResult<TrustedTouchpointDto>> GetPagedAsync(TrustedTouchpointFilters filters)
        {
            var query = _context.TrustedTouchpoints
                .Include(t => t.Company)
                .Where(t => !t.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.SystemName))
                query = query.Where(t => t.SystemName.Contains(filters.SystemName));

            if (filters.CompanyId.HasValue)
                query = query.Where(t => t.CompanyId == filters.CompanyId.Value);

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(t => t.SystemName)
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(t => MapToDto(t))
                .ToListAsync();

            return new PaginatedResult<TrustedTouchpointDto>
            {
                Items = items,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalItems = totalItems
            };
        }

        /// <inheritdoc />
        public async Task<TrustedTouchpointDto?> GetByIdAsync(Guid id)
        {
            var entity = await _context.TrustedTouchpoints
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            return entity == null ? null : MapToDto(entity);
        }

        /// <inheritdoc />
        public async Task<TrustedTouchpointCreatedDto> AddAsync(TrustedTouchpointCreateRequest request)
        {
            try
            {
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == request.CompanyId && !c.IsDeleted);

                if (company == null)
                    throw new InvalidOperationException("Target company does not exist.");

                var tpSlug = NameHelper.EnsureSlug(request.DisplayName);
                var fullName = $"{tpSlug}@{company.SystemName}";

                var exists = await _context.TrustedTouchpoints.AnyAsync(t =>
                    t.SystemName == fullName && !t.IsDeleted);
                if (exists)
                    throw new InvalidOperationException($"Touchpoint '{fullName}' already exists.");

                string? generatedSecret = null;
                string? hashedSecret = null;

                if (!request.UseCompanySecret)
                {
                    generatedSecret = SecretGenerator.Generate();
                    if (string.IsNullOrWhiteSpace(generatedSecret))
                        throw new InvalidOperationException("Failed to generate secret.");

                    hashedSecret = EncryptionHelper.EncryptSecret(generatedSecret);
                }

                var entity = new TrustedTouchpointEntity
                {
                    Id = Guid.NewGuid(),
                    CompanyId = request.CompanyId,
                    SystemName = fullName,
                    DisplayName = request.DisplayName,
                    Secret = hashedSecret,
                    UseCompanySecret = request.UseCompanySecret,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    Mode = Enum.Parse<TouchpointMode>(request.Mode),
                    SignatureType = Enum.Parse<SignatureType>(request.SignatureType),
                    Description = request.Description?.Trim(),
                    IsDeleted = false
                };

                _context.TrustedTouchpoints.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created touchpoint '{Name}' for Company {CompanyId}", entity.SystemName, entity.CompanyId);

                return new TrustedTouchpointCreatedDto
                {
                    Id = entity.Id,
                    SystemName = entity.SystemName,
                    DisplayName = entity.DisplayName,
                    CompanyName = company.SystemName,
                    Mode = entity.Mode.ToString(),
                    SignatureType = entity.SignatureType.ToString(),
                    IsActive = entity.IsActive,
                    UseCompanySecret = entity.UseCompanySecret,
                    CreatedAt = entity.CreatedAt,
                    SecretPlaintext = generatedSecret ?? "(using company secret)"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create trusted touchpoint.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TrustedTouchpointUpdateRequest request)
        {
            try
            {
                var entity = await _context.TrustedTouchpoints
                    .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted);

                if (entity == null)
                    throw new KeyNotFoundException($"Trusted touchpoint with ID '{request.Id}' not found.");

                entity.DisplayName = request.DisplayName;
                entity.UseCompanySecret = request.UseCompanySecret;
                entity.IsActive = request.IsActive;
                entity.Mode = Enum.Parse<TouchpointMode>(request.Mode);
                entity.SignatureType = Enum.Parse<SignatureType>(request.SignatureType);
                entity.Description = request.Description?.Trim();

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated touchpoint '{Id}'", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update trusted touchpoint '{Id}'", request.Id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _context.TrustedTouchpoints
                    .Include(t => t.OutgoingPolicies)
                    .Include(t => t.IncomingPolicies)
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (entity == null)
                    return;

                if (entity.OutgoingPolicies.Any() || entity.IncomingPolicies.Any())
                    throw new InvalidOperationException("Cannot delete touchpoint with linked policies.");

                entity.IsDeleted = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Soft-deleted touchpoint '{Id}'", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete touchpoint '{Id}'", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<int> GetTotalTouchpointCountAsync()
        {
            return await _context.TrustedTouchpoints.CountAsync(t => !t.IsDeleted);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            return await _context.TrustedTouchpoints.AnyAsync(t =>
                t.SystemName == name.ToLowerInvariant() &&
                !t.IsDeleted &&
                (!excludeId.HasValue || t.Id != excludeId.Value));
        }

        /// <inheritdoc />
        public async Task<List<TrustedTouchpointDto>> GetByCompanyAsync(Guid companyId)
        {
            try
            {
                return await _context.TrustedTouchpoints
                    .Where(t => t.CompanyId == companyId && !t.IsDeleted)
                    .OrderBy(t => t.SystemName)
                    .Select(t => new TrustedTouchpointDto
                    {
                        Id = t.Id,
                        SystemName = t.SystemName,
                        DisplayName = t.DisplayName,
                        CompanyId = t.CompanyId,
                        CompanyName = t.Company != null ? t.Company.SystemName : "(unknown)",
                        UseCompanySecret = t.UseCompanySecret,
                        IsActive = t.IsActive,
                        Mode = t.Mode,
                        SignatureType = t.SignatureType,
                        Description = t.Description,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load touchpoints for company '{CompanyId}'", companyId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<SecretRotationResult?> RotateSecretAsync(Guid id)
        {
            try
            {
                var entity = await _context.TrustedTouchpoints.FindAsync(id);
                if (entity == null || entity.IsDeleted)
                    return null;

                if (entity.UseCompanySecret)
                    throw new InvalidOperationException("Cannot rotate secret for touchpoint using company secret.");

                var newSecret = SecretGenerator.Generate();
                if (string.IsNullOrWhiteSpace(newSecret))
                    throw new InvalidOperationException("Failed to generate new secret.");

                entity.Secret = EncryptionHelper.EncryptSecret(newSecret);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Rotated secret for touchpoint '{Id}'", entity.Id);

                return new SecretRotationResult
                {
                    EntityId = entity.Id,
                    OwnerType = SecretOwnerType.TrustedTouchpoint,
                    SecretPlaintext = newSecret
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rotate secret for touchpoint '{Id}'", id);
                throw;
            }
        }

        // --- Helpers ---

        /// <summary>
        /// Maps a <see cref="TrustedTouchpointEntity"/> to its corresponding DTO.
        /// </summary>
        /// <param name="t">The touchpoint entity to map.</param>
        /// <returns>A fully populated <see cref="TrustedTouchpointDto"/>.</returns>
        private static TrustedTouchpointDto MapToDto(TrustedTouchpointEntity t)
        {
            return new TrustedTouchpointDto
            {
                Id = t.Id,
                SystemName = t.SystemName,
                DisplayName = t.DisplayName,
                CompanyId = t.CompanyId,
                CompanyName = t.Company?.SystemName ?? "(unknown)",
                UseCompanySecret = t.UseCompanySecret,
                IsActive = t.IsActive,
                Mode = t.Mode,
                SignatureType = t.SignatureType,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            };
        }
    }
}
