namespace LYRA.Server.Models.Shared
{
    public class ConfirmRotateModel
    {
        public string ModalId { get; set; } = "rotateModal";

        public string ObjectDisplayName { get; set; } = default!;

        public string Handler { get; set; } = "Rotate";

        public Guid ObjectId { get; set; }

        public string Page { get; set; } = "";

        public string Message { get; set; } = "rotate the secret";

        public string Icon { get; set; } = "bi-arrow-clockwise";

        public string? Highlight { get; set; } = "warning";
    }
}
