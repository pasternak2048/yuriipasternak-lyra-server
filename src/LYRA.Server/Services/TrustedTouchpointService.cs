using LYRA.Server.Data;
using LYRA.Server.Entities;
using LYRA.Server.Enums;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.Shared;
using LYRA.Server.Models.TrustedTouchpoint;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities;
using LYRA.Server.Utilities.Security;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services
{
    public class TrustedTouchpointService : ITrustedTouchpointService
    {
        private readonly LyraDbContext _context;
        private readonly ILogger<TrustedTouchpointService> _logger;

        public TrustedTouchpointService(LyraDbContext context, ILogger<TrustedTouchpointService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedResult<TrustedTouchpointDto>> GetPagedAsync(TrustedTouchpointFilters filters)
        {
            var query = _context.TrustedTouchpoints
                .Include(t => t.Company)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Name))
                query = query.Where(t => t.Name.Contains(filters.Name));

            if (filters.CompanyId.HasValue)
                query = query.Where(t => t.CompanyId == filters.CompanyId.Value);

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(t => t.Name)
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

        public async Task<TrustedTouchpointDto?> GetByIdAsync(Guid id)
        {
            var entity = await _context.TrustedTouchpoints
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t => t.Id == id);

            return entity == null ? null : MapToDto(entity);
        }

        public async Task<TrustedTouchpointCreatedDto> AddAsync(TrustedTouchpointCreateRequest request)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == request.CompanyId);
            if (company == null)
                throw new InvalidOperationException("Target company does not exist.");

            // Generate system name: {slugified-touchpoint}@{slugified-company}
            var tpSlug = NameHelper.EnsureSlug(request.DisplayName);
            var fullName = $"{tpSlug}@{company.Name}";

            var exists = await _context.TrustedTouchpoints.AnyAsync(t => t.Name == fullName);
            if (exists)
                throw new InvalidOperationException($"A touchpoint with name '{fullName}' already exists.");

            string? generatedSecret = null;
            string? hashedSecret = null;

            if (!request.UseCompanySecret)
            {
                generatedSecret = SecretGenerator.Generate();
                if (string.IsNullOrWhiteSpace(generatedSecret))
                    throw new InvalidOperationException("Failed to generate secret.");

                hashedSecret = HashHelper.HashSecret(generatedSecret);
            }

            if (!Enum.TryParse<TouchpointMode>(request.Mode, out var parsedMode))
                throw new ArgumentException("Invalid Touchpoint mode.");

            if (!Enum.TryParse<SignatureType>(request.SignatureType, out var parsedSignatureType))
                throw new ArgumentException("Invalid Signature type.");

            var entity = new TrustedTouchpointEntity
            {
                Id = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                Name = fullName,
                DisplayName = request.DisplayName,
                Secret = hashedSecret,
                UseCompanySecret = request.UseCompanySecret,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                Mode = parsedMode,
                SignatureType = parsedSignatureType,
                Description = request.Description?.Trim()
            };

            _context.TrustedTouchpoints.Add(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created touchpoint '{Name}' for Company {CompanyId}", entity.Name, entity.CompanyId);

            return new TrustedTouchpointCreatedDto
            {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                CompanyName = company.Name,
                Mode = entity.Mode.ToString(),
                SignatureType = entity.SignatureType.ToString(),
                IsActive = entity.IsActive,
                UseCompanySecret = entity.UseCompanySecret,
                CreatedAt = entity.CreatedAt,
                SecretPlaintext = generatedSecret ?? "(using company secret)"
            };
        }

        public async Task UpdateAsync(TrustedTouchpointUpdateRequest request)
        {
            var entity = await _context.TrustedTouchpoints.FindAsync(request.Id);
            if (entity == null)
                throw new KeyNotFoundException($"Trusted touchpoint with ID '{request.Id}' not found.");

            // Name is immutable

            entity.DisplayName = request.DisplayName;
            entity.UseCompanySecret = request.UseCompanySecret;
            entity.IsActive = request.IsActive;

            if (!Enum.TryParse<TouchpointMode>(request.Mode, out var parsedMode))
                throw new ArgumentException("Invalid Touchpoint mode.");

            if (!Enum.TryParse<SignatureType>(request.SignatureType, out var parsedSignatureType))
                throw new ArgumentException("Invalid Signature type.");

            entity.Mode = parsedMode;
            entity.SignatureType = parsedSignatureType;
            entity.Description = request.Description?.Trim();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated touchpoint '{Id}'", entity.Id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.TrustedTouchpoints
                .Include(t => t.OutgoingPolicies)
                .Include(t => t.IncomingPolicies)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null) return;

            if (entity.OutgoingPolicies.Any() || entity.IncomingPolicies.Any())
                throw new InvalidOperationException("Cannot delete touchpoint with linked access policies.");

            _context.TrustedTouchpoints.Remove(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted touchpoint '{Id}'", entity.Id);
        }

        public async Task<int> GetTotalTouchpointCountAsync()
        {
            return await _context.TrustedTouchpoints.CountAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var normalized = name.ToLowerInvariant();

            return await _context.TrustedTouchpoints.AnyAsync(c =>
                c.Name == normalized &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<List<TrustedTouchpointDto>> GetByCompanyAsync(Guid companyId)
        {
            return await _context.TrustedTouchpoints
                .Where(t => t.CompanyId == companyId)
                .OrderBy(t => t.Name)
                .Select(t => MapToDto(t))
                .ToListAsync();
        }

        public async Task<SecretRotationResult?> RotateSecretAsync(Guid touchpointId)
        {
            var touchpoint = await _context.TrustedTouchpoints.FindAsync(touchpointId);
            if (touchpoint == null)
                return null;

            if (touchpoint.UseCompanySecret)
                throw new InvalidOperationException("Cannot rotate secret for touchpoint using company secret.");

            var newSecret = SecretGenerator.Generate();
            if (string.IsNullOrWhiteSpace(newSecret))
                throw new InvalidOperationException("Failed to generate new secret.");

            touchpoint.Secret = HashHelper.HashSecret(newSecret);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rotated secret for touchpoint '{Id}'", touchpoint.Id);

            return new SecretRotationResult
            {
                EntityId = touchpoint.Id,
                OwnerType = SecretOwnerType.TrustedTouchpoint,
                SecretPlaintext = newSecret
            };
        }

        // --- Helpers ---

        private static TrustedTouchpointDto MapToDto(TrustedTouchpointEntity t)
        {
            return new TrustedTouchpointDto
            {
                Id = t.Id,
                Name = t.Name,
                DisplayName = t.DisplayName,
                CompanyId = t.CompanyId,
                CompanyName = t.Company?.Name ?? "(unknown)",
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
