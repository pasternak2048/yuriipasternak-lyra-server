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

        public CompanyService(LyraDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyDto>> GetLightweightAsync()
        {
            return await _context.Companies
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
                query = query.Where(c =>
                    c.Name.Contains(filters.Name) ||
                    c.DisplayName != null && c.DisplayName.Contains(filters.Name));
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
            if (entity == null) return null;

            return new CompanyDto
            {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }

        /// <summary>
        /// Creates a new company. The machine-readable Name is auto-generated from DisplayName.
        /// </summary>
        public async Task<CompanyCreatedDto> AddAsync(CompanyCreateRequest request)
        {
            var normalizedName = NameHelper.EnsureSlug(request.DisplayName, "company");

            var exists = await ExistsByNameAsync(normalizedName);
            if (exists)
                throw new InvalidOperationException($"A company with name '{normalizedName}' already exists.");

            string? generatedSecret = null;

            generatedSecret = SecretGenerator.Generate();

            if (string.IsNullOrWhiteSpace(generatedSecret))
                throw new InvalidOperationException("Failed to generate secret.");
            
            var entity = new CompanyEntity
            {
                Id = Guid.NewGuid(),
                Name = normalizedName,
                DisplayName = request.DisplayName,
                Secret = HashHelper.HashSecret(generatedSecret),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(entity);
            await _context.SaveChangesAsync();

            return new CompanyCreatedDto
            {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                SecretPlaintext = generatedSecret
            };
        }

        /// <summary>
        /// Updates an existing company and regenerates its Name from the DisplayName.
        /// </summary>
        public async Task UpdateAsync(CompanyUpdateRequest request)
        {
            var normalizedName = NameHelper.EnsureSlug(request.DisplayName, "company");

            var exists = await ExistsByNameAsync(normalizedName, request.Id);

            if (exists)
                throw new InvalidOperationException($"A company with name '{normalizedName}' already exists.");

            var entity = await _context.Companies.FindAsync(request.Id);
            if (entity == null) return;

            entity.Name = normalizedName;
            entity.DisplayName = request.DisplayName;
            entity.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Companies.FindAsync(id);
            if (entity != null)
            {
                _context.Companies.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalCompanyCountAsync()
        {
            return await _context.Companies.CountAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var normalized = name.ToLowerInvariant();

            return await _context.Companies.AnyAsync(c =>
                c.Name == normalized &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<SecretRotationResult?> RotateSecretAsync(Guid companyId)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if (company == null) return null;

            var newSecret = SecretGenerator.Generate();
            company.Secret = HashHelper.HashSecret(newSecret);

            await _context.SaveChangesAsync();

            return new SecretRotationResult
            {
                EntityId = company.Id,
                OwnerType = SecretOwnerType.Company,
                SecretPlaintext = newSecret
            };
        }
    }
}
