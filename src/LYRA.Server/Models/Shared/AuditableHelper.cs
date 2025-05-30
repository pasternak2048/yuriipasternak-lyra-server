namespace LYRA.Server.Models.Shared
{
    public static class AuditableHelper
    {
        public static void ApplyCreateAudit<T>(T entity, Guid? userId) where T : IAuditableEntity
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = userId;
        }

        public static void ApplyModifyAudit<T>(T entity, Guid? userId) where T : IAuditableEntity
        {
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = userId;
        }

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
