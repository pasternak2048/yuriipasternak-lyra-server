using LYRA.Server.Enums;
using LYRA.Server.Models.Pagination;

namespace LYRA.Server.Models.AccessPolicy
{
    public class AccessPolicyFilters : PaginationFilter
    {
        public string? CallerSystemName { get; set; }

        public string? TargetSystemName { get; set; }

        public string? Operation { get; set; }

        public AccessContext? Context { get; set; }
    }
}
