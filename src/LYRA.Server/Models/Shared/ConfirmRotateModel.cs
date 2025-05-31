namespace LYRA.Server.Models.Shared
{
    /// <summary>
    /// Represents the model used for rendering a confirmation modal before rotating a secret or performing a similar sensitive operation.
    /// </summary>
    public class ConfirmRotateModel
    {
        /// <summary>
        /// The HTML ID of the modal. Default is "rotateModal".
        /// </summary>
        public string ModalId { get; set; } = "rotateModal";

        /// <summary>
        /// The display name of the object for which the operation will be performed (e.g. Touchpoint name).
        /// </summary>
        public string ObjectDisplayName { get; set; } = default!;

        /// <summary>
        /// The handler method name to invoke when the operation is confirmed. Default is "Rotate".
        /// </summary>
        public string Handler { get; set; } = "Rotate";

        /// <summary>
        /// The unique identifier of the object to be modified.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The name of the Razor Page that will handle the operation.
        /// </summary>
        public string Page { get; set; } = "";

        /// <summary>
        /// The message displayed in the confirmation dialog.
        /// </summary>
        public string Message { get; set; } = "rotate the secret";

        /// <summary>
        /// The Bootstrap icon class to display in the modal (e.g. "bi-arrow-clockwise").
        /// </summary>
        public string Icon { get; set; } = "bi-arrow-clockwise";

        /// <summary>
        /// Optional highlight level (e.g. "warning", "danger") used for styling.
        /// </summary>
        public string? Highlight { get; set; } = "warning";
    }
}
