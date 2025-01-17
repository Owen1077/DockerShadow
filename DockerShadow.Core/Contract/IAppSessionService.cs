namespace DockerShadow.Core.Contract
{
    public interface IAppSessionService
    {
        Task<bool> CreateSession(string sessionId, string username);
        bool DeleteSession(string username);
        Task<bool> ValidateSession(string sessionId, string username);
    }
}
