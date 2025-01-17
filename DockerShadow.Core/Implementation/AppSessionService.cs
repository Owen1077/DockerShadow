using DockerShadow.Core.Cache.Services;
using DockerShadow.Core.Contract;
using DockerShadow.Domain.Settings;
using Microsoft.Extensions.Options;

namespace DockerShadow.Core.Implementation
{
    public class AppSessionService : IAppSessionService
    {
        private readonly ICacheService _cacheService;
        private readonly JWTSettings _jwtSettings;

        public AppSessionService(ICacheService cacheService,
            IOptions<JWTSettings> jwtSettings)
        {
            _cacheService = cacheService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<bool> CreateSession(string sessionId, string username)
        {
            var sessionKey = $"session_{username}";
            // If there is an existing session, the command below will overwrite it
            // Make it expire with the jwt token
            await _cacheService.SetAsync(sessionKey, sessionId, TimeSpan.FromMinutes(_jwtSettings.DurationInMinutes));
            return true;
        }

        public bool DeleteSession(string username)
        {
            var sessionKey = $"session_{username}";
            _cacheService.Remove(sessionKey);

            return true;
        }

        public async Task<bool> ValidateSession(string sessionId, string username)
        {
            var sessionKey = $"session_{username}";
            var existingSessionId = await _cacheService.GetAsync<string>(sessionKey);

            if (sessionId == existingSessionId)
            {
                return true;
            }
            return false;
        }
    }
}
