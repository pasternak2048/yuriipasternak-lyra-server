using Microsoft.AspNetCore.Mvc.Rendering;

namespace LYRA.Server.Utilities
{
    /// <summary>
    /// Helper class for working with enums in UI scenarios.
    /// Provides methods for generating select list items from enum values.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Generates a list of <see cref="SelectListItem"/> objects from the specified enum type.
        /// Suitable for use in dropdowns or selection components in Razor Pages or MVC views.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to convert.</typeparam>
        /// <returns>A list of select items with both value and text set to enum names.</returns>
        public static List<SelectListItem> GetSelectList<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                }).ToList();
        }
    }
}
