using DockerShadow.Core.Contract;
using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;
using DockerShadow.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DockerShadow.Core.Implementation
{
    public class APIImplementation : IAPIImplementation
    {
        private readonly IClientFactory _clientFactory;
        private readonly ILogger<APIImplementation> _logger;
        private readonly ExternalApiOptions _externalApiOptions;

        public APIImplementation(IClientFactory clientFactory,
            ILogger<APIImplementation> logger,
            IOptions<ExternalApiOptions> externalApiOptions)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _externalApiOptions = externalApiOptions.Value;
        }

        public async Task<SendMailResponse> SendMail(SendMailRequest request)
        {
            var headers = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Authorization", _externalApiOptions.MailAuthorizationKey),
            };

            SendMailResponse response = await _clientFactory.PostDataAsync<SendMailResponse, SendMailRequest>(_externalApiOptions.SendMail, request, headers);

            if (!response.succeeded)
            {
                _logger.LogError($"An error occured while calling {nameof(SendMail)} for {request.Recipient}.");
                _logger.LogError(JsonConvert.SerializeObject(response));
            }
            return response;
        }
    }
}
