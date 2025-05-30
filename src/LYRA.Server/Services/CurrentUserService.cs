using LYRA.Server.Services.Interfaces;
using System.Security.Claims;

namespace LYRA.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

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
