using DockerShadow.Core.Cache.Options;
using DockerShadow.Core.Cache.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DockerShadow.Core.Cache
{
    public static class RegistrationExtension
    {
        public static void AddCustomCache(this IServiceCollection services, IConfiguration config)
        {
            var cacheOptions = new CacheOptions();
            var cacheSection = config.GetSection("CacheOptions");

            cacheSection.Bind(cacheOptions);
            services.Configure<CacheOptions>(cacheSection);

            switch (cacheOptions.CacheProvider)
            {
                case "InMemory":
                    services.AddInMemoryCache();
                    break;
                case "Redis":
                    services.AddRedisCache(cacheOptions);
                    break;
                default:
                    throw new Exception("Cache provider is not supported");
            }
        }

        private static void AddInMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();

            if (services.Contains(ServiceDescriptor.Transient<ICacheService, InMemoryCacheService>()))
            {
                return;
            }

            services.AddTransient<ICacheService, InMemoryCacheService>();
        }

        private static void AddRedisCache(this IServiceCollection services, CacheOptions cacheOptions)
        {
            if (services.Contains(ServiceDescriptor.Transient<ICacheService, DistributedCacheService>()))
            {
                return;
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = cacheOptions.RedisCacheOptions.Prefix;
                options.ConfigurationOptions = GetRedisConfigurationOptions(cacheOptions.RedisCacheOptions);
            });

            services.AddTransient<ICacheService, DistributedCacheService>();
        }

        private static ConfigurationOptions GetRedisConfigurationOptions(RedisCacheOptions redisOptions)
        {
            var configurationOptions = new ConfigurationOptions
            {
                ConnectTimeout = redisOptions.ConnectTimeout,
                SyncTimeout = redisOptions.SyncTimeout,
                ConnectRetry = redisOptions.ConnectRetry,
                AbortOnConnectFail = redisOptions.AbortOnConnectFail,
                ReconnectRetryPolicy = new ExponentialRetry(redisOptions.DeltaBackoffMiliseconds),
                KeepAlive = 5,
                Ssl = redisOptions.Ssl
            };

            if (!string.IsNullOrWhiteSpace(redisOptions.Password))
            {
                configurationOptions.Password = redisOptions.Password;
            }

            var endpoints = redisOptions.Url.Split(',');
            foreach (var endpoint in endpoints)
            {
                configurationOptions.EndPoints.Add(endpoint);
            }

            return configurationOptions;
        }
    }
}
