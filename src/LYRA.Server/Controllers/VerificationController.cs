using LYRA.Security.Models.Verify;
using LYRA.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LYRA.Server.Controllers
{
    /// <summary>
    /// API controller for verifying signed requests using digital signatures and trusted touchpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationController : ControllerBase
    {
        private readonly IVerifyService _verifyService;
        private readonly ILogger<VerificationController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationController"/> class.
        /// </summary>
        public VerificationController(
            IVerifyService verifyService,
            ILogger<VerificationController> logger)
        {
            _verifyService = verifyService;
            _logger = logger;
        }

        /// <summary>
        /// Verifies the authenticity and integrity of an incoming request using its digital signature.
        /// </summary>
        /// <param name="request">The signed verification request payload.</param>
        /// <returns>
        /// 200 OK if the signature is valid; 400 Bad Request with an error message otherwise.
        /// </returns>
        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyAsync([FromBody] VerifyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request payload.");

            var response = await _verifyService.Verify(request);

            if (response.IsSuccess)
                return Ok(response);

            _logger.LogWarning("Verification failed: {Message}", response.ErrorMessage);
            return BadRequest(response);
        }
    }
}
