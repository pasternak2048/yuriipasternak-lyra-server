namespace LYRA.Server.Models.AccessPolicy
{
    public class AccessRule
    {
        public string Method { get; set; } = null!;

        public string PathPattern { get; set; } = null!;
    }
}
