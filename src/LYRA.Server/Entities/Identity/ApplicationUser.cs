using Microsoft.AspNetCore.Identity;

namespace LYRA.Server.Entities.Identity
{
    /// <summary>
    /// Represents the application user entity extending ASP.NET Core IdentityUser.
    /// Can be extended with additional properties related to the authenticated user.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Currently no additional properties are defined.
        // Add custom user properties here if needed in the future.
    }
}
