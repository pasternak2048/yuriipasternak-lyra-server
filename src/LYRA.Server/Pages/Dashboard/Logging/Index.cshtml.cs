using LYRA.Server.Models.Logging;
using LYRA.Server.Services.Logging.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LYRA.Server.Pages.Dashboard.Logging
{
    /// <summary>
    /// Displays recent system logs and updates the view with real-time messages via SignalR.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogService _logService;

        /// <summary>
        /// Most recent log entries to render on initial page load.
        /// </summary>
        public List<LogEntryDto> Logs { get; private set; } = new();

        public IndexModel(ILogService logService)
        {
            _logService = logService;
        }

        public async Task OnGetAsync()
        {
            Logs = await _logService.GetRecentAsync(100);
        }
    }
}
