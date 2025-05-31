using LYRA.Server.Services.Interfaces;
using System.Security.Claims;

namespace LYRA.Server.Services
{
    /// <summary>
    /// Provides access to the ID of the currently authenticated user based on the HTTP context.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Accessor to retrieve the current HTTP context.</param>
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public Guid? UserId
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext?.User?.Identity?.IsAuthenticated != true)
                    return null;

                var userIdClaim =
                    httpContext.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    httpContext.User.FindFirst("sub");

                if (userIdClaim == null)
                    return null;

                return Guid.TryParse(userIdClaim.Value, out var id)
                    ? id
                    : null;
            }
        }
    }
}
