using LYRA.Server.Models.Shared;
using LYRA.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LYRA.Server.Data.Auditing
{
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        private void ApplyAudit(DbContext? context)
        {
            if (context == null) return;

            var userId = _currentUserService.UserId ?? Guid.Empty;
            var now = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is IAuditableEntity auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditable.CreatedAt = now;
                        if (auditable.CreatedBy == null || auditable.CreatedBy == Guid.Empty)
                            auditable.CreatedBy = userId;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        auditable.ModifiedAt = now;
                        if (auditable.ModifiedBy == null || auditable.ModifiedBy == Guid.Empty)
                            auditable.ModifiedBy = userId;
                    }
                }
            }
        }
    }
}
