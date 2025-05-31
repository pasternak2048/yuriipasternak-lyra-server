namespace LYRA.Server.Models.Shared
{
    /// <summary>
    /// Defines audit fields that should be tracked for any entity that supports creation and modification auditing.
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// The UTC timestamp when the entity was created.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// The ID of the user who created the entity, if available.
        /// </summary>
        Guid? CreatedBy { get; set; }

        /// <summary>
        /// The UTC timestamp when the entity was last modified, if applicable.
        /// </summary>
        DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// The ID of the user who last modified the entity, if available.
        /// </summary>
        Guid? ModifiedBy { get; set; }
    }
}
