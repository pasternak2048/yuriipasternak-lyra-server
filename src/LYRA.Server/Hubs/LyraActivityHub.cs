using Microsoft.AspNetCore.SignalR;

namespace LYRA.Server.Hubs
{
    /// <summary>
    /// SignalR hub that broadcasts live log entries to connected clients.
    /// Used by the admin console to observe activity in real time.
    /// </summary>
    public class LyraActivityHub : Hub
    {
        // Currently no server-to-client methods needed
        // All messages are pushed from LogService via IHubContext
    }
}
