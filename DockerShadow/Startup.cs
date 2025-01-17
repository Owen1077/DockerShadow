using DockerShadow.Core.Cache;
using DockerShadow.Infrastructure.Extension;
using Serilog;

namespace DockerShadow
{
    public class Startup
    {
        private readonly IConfigurationRoot configRoot;

        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            Configuration = configuration;

            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            configRoot = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransientServices();

            services.AddScopedServices();

            services.AddSingletonServices();

            services.AddCustomCache(Configuration);

            services.AddDatabaseContext(Configuration);

            services.AddRepositoryServices();

            services.AddJwtIdentityService(Configuration);

            services.AddCustomAutoMapper();

            services.AddSwaggerOpenAPI();

            services.AddCustomHostedServices();

            services.AddCustomOptions();

            services.AddCustomControllers();

            services.AddRequestRateLimiter(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory log)
        {
            app.ConfigureCors(Configuration);

            app.ConfigureCustomExceptionMiddleware();

            app.ConfigureSwagger(env);

            log.AddSerilog();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
