namespace LYRA.Server.Models.Shared
{
    /// <summary>
    /// Helper class for applying audit information to entities implementing IAuditableEntity.
    /// </summary>
    public static class AuditableHelper
    {
        /// <summary>
        /// Applies creation audit information to the given entity.
        /// Sets CreatedAt to current UTC time and CreatedBy to the provided user ID.
        /// </summary>
        /// <typeparam name="T">Type implementing IAuditableEntity</typeparam>
        /// <param name="entity">The entity to apply audit info to</param>
        /// <param name="userId">The ID of the user who created the entity</param>
        public static void ApplyCreateAudit<T>(T entity, Guid? userId) where T : IAuditableEntity
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = userId;
        }

        /// <summary>
        /// Applies modification audit information to the given entity.
        /// Sets ModifiedAt to current UTC time and ModifiedBy to the provided user ID.
        /// </summary>
        /// <typeparam name="T">Type implementing IAuditableEntity</typeparam>
        /// <param name="entity">The entity to apply audit info to</param>
        /// <param name="userId">The ID of the user who modified the entity</param>
        public static void ApplyModifyAudit<T>(T entity, Guid? userId) where T : IAuditableEntity
        {
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = userId;
        }

        /// <summary>
        /// Applies audit information based on whether the entity is new or being modified.
        /// </summary>
        /// <typeparam name="T">Type implementing IAuditableEntity</typeparam>
        /// <param name="entity">The entity to apply audit info to</param>
        /// <param name="userId">The ID of the user performing the operation</param>
        /// <param name="isNew">True if the entity is new (creation), false if modification</param>
        public static void ApplyAudit<T>(T entity, Guid? userId, bool isNew) where T : IAuditableEntity
        {
            if (isNew)
            {
                ApplyCreateAudit(entity, userId);
            }
            else
            {
                ApplyModifyAudit(entity, userId);
            }
        }
    }
}
