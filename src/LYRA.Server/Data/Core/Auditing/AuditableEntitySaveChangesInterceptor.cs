using LYRA.Server.Models.Shared;
using LYRA.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LYRA.Server.Data.Core.Auditing
{
    /// <summary>
    /// Interceptor that automatically updates audit properties
    /// (CreatedAt, CreatedBy, ModifiedAt, ModifiedBy) on entities
    /// implementing <see cref="IAuditableEntity"/> during SaveChanges.
    /// </summary>
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditableEntitySaveChangesInterceptor"/> class.
        /// </summary>
        /// <param name="currentUserService">Service to get the current user ID.</param>
        public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Called asynchronously before changes are saved to the database.
        /// Applies audit information to entities.
        /// </summary>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// Called synchronously before changes are saved to the database.
        /// Applies audit information to entities.
        /// </summary>
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        /// <summary>
        /// Applies audit properties (CreatedAt, CreatedBy, ModifiedAt, ModifiedBy)
        /// to entities tracked by the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The database context.</param>
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
