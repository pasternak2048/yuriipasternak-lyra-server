namespace LYRA.Server.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
