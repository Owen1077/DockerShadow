using DockerShadow.Core.Contract;
using DockerShadow.Core.DTO.Request;
using DockerShadow.Domain.Settings;
using Microsoft.Extensions.Options;

namespace DockerShadow.Core.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly AdminOptions _adminOptions;
        private readonly ITemplateService _templateService;
        private readonly IQueueManager _queueManager;

        public NotificationService(IOptions<AdminOptions> adminOptions,
            ITemplateService templateService,
            IQueueManager queueManager)
        {
            _adminOptions = adminOptions.Value;
            _templateService = templateService;
            _queueManager = queueManager;
        }

        /**
         * GENERAL COMMENTS ON THIS SERVICE
         * 
        **/
        public async Task SendPasswordResetToken(string userName, string url, string firstName, string email)
        {
            WelcomeEmailTemplateRequest templateRequest = new WelcomeEmailTemplateRequest()
            {
                UserName = userName,
                Name = firstName,
                Url = url
            };
            var emailRequest = new SendMailRequest()
            {
                From = _adminOptions.BroadcastEmail,
                Recipient = email,
                Subject = "Coronation Portal Password Reset Token",
                Content = _templateService.GenerateHtmlStringFromViewsAsync("WelcomeNotification", templateRequest)
            };

            await _queueManager.PushEmailAsync(emailRequest);
        }
    }
}
