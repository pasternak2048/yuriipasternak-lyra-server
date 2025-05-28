using Microsoft.AspNetCore.Mvc.Rendering;

namespace LYRA.Server.Utilities
{
    public static class EnumHelper
    {
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
