namespace DockerShadow.Core.Contract
{
    public interface INotificationService
    {
        Task SendPasswordResetToken(string userName, string url, string firstName, string email);
    }
}
