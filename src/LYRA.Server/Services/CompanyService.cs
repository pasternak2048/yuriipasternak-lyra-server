using LYRA.Server.Data;
using LYRA.Server.Entities;
using LYRA.Server.Enums;
using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Models.Shared;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities;
using LYRA.Server.Utilities.Security;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly LyraDbContext _context;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(LyraDbContext context, ILogger<CompanyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<CompanyDto>> GetLightweightAsync()
        {
            return await _context.Companies
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    DisplayName = c.DisplayName
                })
                .ToListAsync();
        }

        public async Task<PaginatedResult<CompanyDto>> GetPagedAsync(CompanyFilters filters)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                var nameFilter = filters.Name.Trim().ToLowerInvariant();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(nameFilter) ||
                    (c.DisplayName != null && c.DisplayName.ToLower().Contains(nameFilter)));
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.Name)
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    DisplayName = c.DisplayName,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return new PaginatedResult<CompanyDto>
            {
                Items = items,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalItems = totalItems
            };
        }

        public async Task<CompanyDto?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Companies.FindAsync(id);
            return entity == null
                ? null
                : MapToDto(entity);
        }

        public async Task<CompanyCreatedDto> AddAsync(CompanyCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                throw new ArgumentException("DisplayName is required.", nameof(request.DisplayName));

            var normalizedName = NormalizeName(request.DisplayName);
            var exists = await _context.Companies.AnyAsync(c => c.Name == normalizedName);
            if (exists)
                throw new InvalidOperationException($"A company with name '{normalizedName}' already exists.");

            var secretPlaintext = SecretGenerator.Generate();
            if (string.IsNullOrWhiteSpace(secretPlaintext))
                throw new InvalidOperationException("Failed to generate secret.");

            var entity = new CompanyEntity
            {
                Id = Guid.NewGuid(),
                Name = normalizedName,
                DisplayName = request.DisplayName,
                Secret = HashHelper.HashSecret(secretPlaintext),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new company: {CompanyName} ({CompanyId})", entity.Name, entity.Id);

            return new CompanyCreatedDto
            {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                SecretPlaintext = secretPlaintext
            };
        }

        public async Task UpdateAsync(CompanyUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                throw new ArgumentException("DisplayName is required.", nameof(request.DisplayName));

            var entity = await _context.Companies.FindAsync(request.Id);
            if (entity == null)
                throw new KeyNotFoundException($"Company with ID '{request.Id}' not found.");

            entity.DisplayName = request.DisplayName;
            entity.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated company: {CompanyId}", entity.Id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Companies
                .Include(c => c.TrustedTouchpoints)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null) return;

            if (entity.TrustedTouchpoints.Any())
                throw new InvalidOperationException("Cannot delete company with existing trusted touchpoints.");

            _context.Companies.Remove(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted company: {CompanyId}", id);
        }

        public async Task<int> GetTotalCompanyCountAsync()
        {
            return await _context.Companies.CountAsync();
        }

        public async Task<SecretRotationResult?> RotateSecretAsync(Guid companyId)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if (company == null) return null;

            var newSecret = SecretGenerator.Generate();
            if (string.IsNullOrWhiteSpace(newSecret))
                throw new InvalidOperationException("Failed to generate secret.");

            var oldSecret = company.Secret;
            company.Secret = HashHelper.HashSecret(newSecret);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rotated secret for company: {CompanyId}", company.Id);

            return new SecretRotationResult
            {
                EntityId = company.Id,
                OwnerType = SecretOwnerType.Company,
                SecretPlaintext = newSecret
            };
        }

        public async Task<bool> ExistsByDisplayNameAsync(string displayName, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name is required.", nameof(displayName));

            var normalized = NormalizeName(displayName);

            return await _context.Companies.AnyAsync(c =>
                c.Name == normalized &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        // --- Helpers ---

        private static string NormalizeName(string displayName)
        {
            return NameHelper.EnsureSlug(displayName.Trim(), "company");
        }

        private static CompanyDto MapToDto(CompanyEntity entity)
        {
            return new CompanyDto
            {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
