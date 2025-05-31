using LYRA.Server.Data;
using LYRA.Server.Entities;
using LYRA.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LYRA.Server.Services.SecurityVerification
{
    public class SecretProvider : ISecretProvider
    {
        private readonly LyraDbContext _context;

        public SecretProvider(LyraDbContext context)
        {
            _context = context;
        }

        public async Task<TrustedTouchpointEntity?> GetTouchpointAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            return await _context.TrustedTouchpoints
                .Include(t => t.Company)
                .FirstOrDefaultAsync(t =>
                    t.SystemName == systemName &&
                    !t.IsDeleted &&
                    t.IsActive &&
                    t.Company != null &&
                    t.Company.IsActive);
        }
    }
}
