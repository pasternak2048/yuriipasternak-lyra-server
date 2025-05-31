namespace LYRA.Server.Models.Verify
{
    public class VerifyResponse
    {
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }

        public int StatusCode => IsSuccess ? 200 : 403;
    }
}
