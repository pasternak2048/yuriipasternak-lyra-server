using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.TrustedTouchpoint
{
    /// <summary>
    /// DTO used for updating an existing trusted touchpoint.
    /// Inherits creation fields and adds the unique identifier of the touchpoint.
    /// </summary>
    public class TrustedTouchpointUpdateRequest : TrustedTouchpointCreateRequest
    {
        /// <summary>
        /// ID of the touchpoint being updated.
        /// </summary>
        [Required]
        public Guid Id { get; set; }
    }
}
