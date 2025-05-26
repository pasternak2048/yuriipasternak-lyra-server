using LYRA.Server.Data;
using LYRA.Server.Entities;
using LYRA.Server.Models.Company;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Services.Interfaces;
using LYRA.Server.Utilities;
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
                    Secret = c.Secret,
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
                Secret = entity.Secret,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }

        /// <summary>
        /// Creates a new company. The machine-readable Name is auto-generated from DisplayName.
        /// </summary>
        public async Task AddAsync(CompanyCreateRequest request)
        {
            var entity = new CompanyEntity
            {
                Id = Guid.NewGuid(),
                Name = SlugHelper.Slugify(request.DisplayName),
                DisplayName = request.DisplayName,
                Secret = request.Secret,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing company and regenerates its Name from the DisplayName.
        /// </summary>
        public async Task UpdateAsync(CompanyUpdateRequest request)
        {
            var entity = await _context.Companies.FindAsync(request.Id);
            if (entity == null) return;

            entity.Name = SlugHelper.Slugify(request.DisplayName);
            entity.DisplayName = request.DisplayName;
            entity.Secret = request.Secret;
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
    }
}
