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

        public TrustedTouchpointService(LyraDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a paginated list of trusted touchpoints based on filters
        /// </summary>
        public async Task<PaginatedResult<TrustedTouchpointDto>> GetPagedAsync(TrustedTouchpointFilters filters)
        {
            var query = _context.TrustedTouchpoints.Include(t => t.Company).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                query = query.Where(t => t.Name.Contains(filters.Name));
            }

            if (filters.CompanyId.HasValue)
            {
                query = query.Where(t => t.CompanyId == filters.CompanyId.Value);
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(t => t.Name)
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(t => new TrustedTouchpointDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    DisplayName = t.DisplayName,
                    CompanyName = t.Company.Name,
                    UseCompanySecret = t.UseCompanySecret,
                    IsActive = t.IsActive,
                    Mode = t.Mode,
                    SignatureType = t.SignatureType,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return new PaginatedResult<TrustedTouchpointDto>
            {
                Items = items,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalItems = totalItems
            };
        }

        /// <summary>
        /// Returns details of a single trusted touchpoint by ID
        /// </summary>
        public async Task<TrustedTouchpointDto?> GetByIdAsync(Guid id)
        {
            var entity = await _context.TrustedTouchpoints
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null) return null;

            return new TrustedTouchpointDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                CompanyName = entity.Company.Name,
                UseCompanySecret = entity.UseCompanySecret,
                IsActive = entity.IsActive,
                Mode = entity.Mode,
                SignatureType = entity.SignatureType,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt
            };
        }

        /// <summary>
        /// Adds a new trusted touchpoint
        /// </summary>
        public async Task<TrustedTouchpointCreatedDto> AddAsync(TrustedTouchpointCreateRequest request)
        {
            var normalizedName = NameHelper.EnsureSlug(request.DisplayName, "trusted-touchpoint");

            var exists = await ExistsByCompanyAndNameAsync(request.CompanyId, normalizedName);
            if (exists)
                throw new InvalidOperationException($"A touchpoint with name '{normalizedName}' already exists in this company.");

            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == request.CompanyId);
            if (company == null)
                throw new InvalidOperationException("Target company does not exist.");

            string? generatedSecret = null;
            string? hashedSecret = null;

            if (!request.UseCompanySecret)
            {
                generatedSecret = SecretGenerator.Generate();

                if (string.IsNullOrWhiteSpace(generatedSecret))
                    throw new InvalidOperationException("Failed to generate secret.");

                hashedSecret = HashHelper.HashSecret(generatedSecret);
            }

            var entity = new TrustedTouchpointEntity
            {
                Id = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                Name = normalizedName,
                DisplayName = request.DisplayName,
                Secret = hashedSecret,
                UseCompanySecret = request.UseCompanySecret,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                Mode = Enum.Parse<TouchpointMode>(request.Mode),
                SignatureType = Enum.Parse<SignatureType>(request.SignatureType)
            };

            _context.TrustedTouchpoints.Add(entity);
            await _context.SaveChangesAsync();

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

        /// <summary>
        /// Updates an existing trusted touchpoint
        /// </summary>
        public async Task UpdateAsync(TrustedTouchpointUpdateRequest request)
        {
            var entity = await _context.TrustedTouchpoints.FindAsync(request.Id);
            if (entity == null) return;

            entity.DisplayName = request.DisplayName;
            entity.UseCompanySecret = request.UseCompanySecret;
            entity.IsActive = request.IsActive;
            entity.Mode = Enum.Parse<TouchpointMode>(request.Mode);
            entity.SignatureType = Enum.Parse<SignatureType>(request.SignatureType);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a trusted touchpoint by ID
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.TrustedTouchpoints.FindAsync(id);
            if (entity != null)
            {
                _context.TrustedTouchpoints.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Returns the total number of trusted touchpoints in the system
        /// </summary>
        public async Task<int> GetTotalTouchpointCountAsync()
        {
            return await _context.TrustedTouchpoints.CountAsync();
        }

        /// <summary>
        /// Checks if a Trusted Touchpoint with the given name (normalized) already exists in the database.
        /// Optionally excludes a specific Touchpoint ID from the check (useful for update scenarios).
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var normalized = name.ToLowerInvariant();

            return await _context.TrustedTouchpoints.AnyAsync(c =>
                c.Name == normalized &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        /// <summary>
        /// Checks whether a trusted touchpoint with the given normalized name
        /// already exists for the specified company. This is used to enforce the
        /// uniqueness constraint on (CompanyId, Name) combination.
        /// </summary>
        /// <param name="companyId">ID of the company to check within</param>
        /// <param name="name">Normalized (slugified) name of the touchpoint</param>
        /// <returns>True if a touchpoint with the same name exists in the company</returns>

        public async Task<bool> ExistsByCompanyAndNameAsync(Guid companyId, string name, Guid? excludeId = null)
        {
            return await _context.TrustedTouchpoints.AnyAsync(t =>
                t.CompanyId == companyId &&
                t.Name == name &&
                (!excludeId.HasValue || t.Id != excludeId.Value));
        }

        /// <summary>
        /// Returns all trusted touchpoints that belong to a specific company
        /// </summary>
        public async Task<List<TrustedTouchpointDto>> GetByCompanyAsync(Guid companyId)
        {
            return await _context.TrustedTouchpoints
                .Where(t => t.CompanyId == companyId)
                .OrderBy(t => t.Name)
                .Select(t => new TrustedTouchpointDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    DisplayName = t.DisplayName,
                    CompanyName = t.Company.Name,
                    UseCompanySecret = t.UseCompanySecret,
                    IsActive = t.IsActive,
                    Mode = t.Mode,
                    SignatureType = t.SignatureType,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<SecretRotationResult?> RotateSecretAsync(Guid touchpointId)
        {
            var touchpoint = await _context.TrustedTouchpoints.FindAsync(touchpointId);
            if (touchpoint == null) return null;

            var newSecret = SecretGenerator.Generate();
            touchpoint.Secret = HashHelper.HashSecret(newSecret);

            await _context.SaveChangesAsync();

            return new SecretRotationResult
            {
                EntityId = touchpoint.Id,
                OwnerType = SecretOwnerType.TrustedTouchpoint,
                SecretPlaintext = newSecret
            };
        }
    }
}
