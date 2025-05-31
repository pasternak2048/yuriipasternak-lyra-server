namespace LYRA.Server.Models.Shared
{
    /// <summary>
    /// Represents the model used for rendering a confirmation modal before deleting an object.
    /// </summary>
    public class ConfirmDeleteModel
    {
        /// <summary>
        /// The HTML ID of the modal. Default is "deleteModal".
        /// </summary>
        public string ModalId { get; set; } = "deleteModal";

        /// <summary>
        /// The display name of the object to be shown in the confirmation message.
        /// </summary>
        public string ObjectDisplayName { get; set; } = default!;

        /// <summary>
        /// The name of the Razor Page that will handle the delete request.
        /// </summary>
        public string? Page { get; set; }

        /// <summary>
        /// The unique identifier of the object to be deleted.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The name of the handler method to invoke for the delete operation. Default is "Delete".
        /// </summary>
        public string Handler { get; set; } = "Delete";
    }
}
