using LYRA.Security.Signing;
using System.Net.Http.Json;

namespace LYRA.Client.Core
{
    /// <summary>
    /// Sends verification requests to LYRA.Server over HTTP.
    /// </summary>
    public sealed class VerifyService
    {
        private readonly HttpClient _http;
        private readonly string _verifyPath;

        /// <param name="http">HttpClient with BaseAddress pointing to LYRA.Server.</param>
        /// <param name="verifyPath">Relative path to verify endpoint (default: "api/verification/verify").</param>
        public VerifyService(HttpClient http, string verifyPath = "api/verification/verify")
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _verifyPath = verifyPath.TrimStart('/');
        }

        public async Task<VerifyResponse> VerifyAsync(VerifyRequest request, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync(_verifyPath, request, ct);
            resp.EnsureSuccessStatusCode();

            var result = await resp.Content.ReadFromJsonAsync<VerifyResponse>(cancellationToken: ct);
            return result ?? throw new InvalidOperationException("Empty VerifyResponse.");
        }
    }
}
