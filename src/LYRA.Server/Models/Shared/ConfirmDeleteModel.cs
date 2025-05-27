namespace LYRA.Server.Models.Shared
{
    public class ConfirmDeleteModel
    {
        public string ModalId { get; set; } = "deleteModal";

        public string ObjectDisplayName { get; set; } = default!;

        public string? Page { get; set; }

        public Guid ObjectId { get; set; }

        public string Handler { get; set; } = "Delete";
    }
}
