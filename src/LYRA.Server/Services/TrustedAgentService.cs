using LYRA.Server.Data;
using LYRA.Server.Entities;
using LYRA.Server.Enums;
using LYRA.Server.Models.Agents;
using LYRA.Server.Models.Pagination;
using LYRA.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services
{
    public class TrustedAgentService : ITrustedAgentService
    {
        private readonly LyraDbContext _context;

        public TrustedAgentService(LyraDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<TrustedAgentDto>> GetPagedAsync(TrustedAgentFilters filters)
        {
            var query = _context.TrustedAgents.Include(a => a.Company).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                query = query.Where(a => a.Name.Contains(filters.Name));
            }

            if (filters.CompanyId.HasValue)
            {
                query = query.Where(a => a.CompanyId == filters.CompanyId.Value);
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.Name)
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(a => new TrustedAgentDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    CompanyName = a.Company.Name,
                    IsActive = a.IsActive,
                    Mode = a.Mode.ToString(),
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return new PaginatedResult<TrustedAgentDto>
            {
                Items = items,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalItems = totalItems
            };
        }

        public async Task<TrustedAgentDto?> GetByIdAsync(Guid id)
        {
            var entity = await _context.TrustedAgents.Include(a => a.Company).FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return null;

            return new TrustedAgentDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CompanyName = entity.Company.Name,
                IsActive = entity.IsActive,
                Mode = entity.Mode.ToString(),
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task AddAsync(TrustedAgentCreateRequest request)
        {
            var entity = new TrustedAgentEntity
            {
                Id = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                Name = request.Name,
                Secret = request.Secret,
                UseCompanySecret = request.UseCompanySecret,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                Mode = Enum.Parse<AgentMode>(request.Mode)
            };

            _context.TrustedAgents.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TrustedAgentUpdateRequest request)
        {
            var entity = await _context.TrustedAgents.FindAsync(request.Id);
            if (entity == null) return;

            entity.Name = request.Name;
            entity.Secret = request.Secret;
            entity.UseCompanySecret = request.UseCompanySecret;
            entity.IsActive = request.IsActive;
            entity.Mode = Enum.Parse<AgentMode>(request.Mode);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.TrustedAgents.FindAsync(id);
            if (entity != null)
            {
                _context.TrustedAgents.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalAgentCountAsync()
        {
            return await _context.TrustedAgents.CountAsync();
        }

        public async Task<List<TrustedAgentDto>> GetByCompanyAsync(Guid companyId)
        {
            return await _context.TrustedAgents
                .Where(a => a.CompanyId == companyId)
                .OrderBy(a => a.Name)
                .Select(a => new TrustedAgentDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    CompanyName = a.Company.Name,
                    IsActive = a.IsActive,
                    Mode = a.Mode.ToString(),
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }
    }

}
