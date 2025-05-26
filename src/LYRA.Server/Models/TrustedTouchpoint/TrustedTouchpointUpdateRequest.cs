using System.ComponentModel.DataAnnotations;

namespace LYRA.Server.Models.TrustedTouchpoint
{
    public class TrustedTouchpointUpdateRequest : TrustedTouchpointCreateRequest
    {
        /// <summary>
        /// ID of the touchpoint being updated
        /// </summary>
        [Required]
        public Guid Id { get; set; }
    }
}
