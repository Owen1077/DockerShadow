using AspNetCoreRateLimit;
using DockerShadow.Core.BackgroundServices;
using DockerShadow.Core.Contract;
using DockerShadow.Core.Contract.Repository;
using DockerShadow.Core.Implementation;
using DockerShadow.Core.Repository;
using DockerShadow.Domain.Entities;
using DockerShadow.Domain.Settings;
using DockerShadow.Infrastructure.Configs;
using DockerShadow.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Text;
using IClientFactory = DockerShadow.Core.Contract.IClientFactory;

namespace DockerShadow.Infrastructure.Extension
{
    public static class ConfigureServiceContainer
    {
        public static void AddDatabaseContext(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var databaseOptions = new DatabaseOptions();
            var databaseSection = configuration.GetSection("DatabaseOptions");
            databaseSection.Bind(databaseOptions);

            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(databaseOptions.ConnectionString, b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.MigrationsHistoryTable("MIGRATION_HISTORY", databaseOptions.SchemaName);
                });
            });
        }

        public static void AddTransientServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAPIImplementation, APIImplementation>();
            serviceCollection.AddTransient<IClientFactory, ClientFactory>();
            serviceCollection.AddTransient<IAppSessionService, AppSessionService>();
            serviceCollection.AddTransient<INotificationService, NotificationService>();
            serviceCollection.AddTransient<IFundLogService, FundLogService>();
            serviceCollection.AddTransient<IWithdrawLogService, WithdrawLogService>();
            serviceCollection.AddTransient<IAccountLogService, AccountLogService>();
        }

        public static void AddSingletonServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IQueueManager, QueueManager>();
        }

        public static void AddScopedServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<ITemplateService, TemplateService>();
        }

        public static void AddRepositoryServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddSingleton<IFundLogRepository, FundLogRepository>();
            serviceCollection.AddSingleton<IWithdrawLogRepository, WithdrawLogRepository>();
            serviceCollection.AddSingleton<IAccountLogRepository, AccountLogRepository>();
        }

        public static void AddJwtIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, Role>(opt =>
            {
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5000);
                opt.Lockout.MaxFailedAccessAttempts = 5;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                    o.Events = new JwtBearerEvents()
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            var code = HttpStatusCode.Unauthorized;
                            context.Response.StatusCode = (int)code;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Domain.Common.Response<string>("You are not Authorized", (int)code));
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            var code = HttpStatusCode.Forbidden;
                            context.Response.StatusCode = (int)code;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Domain.Common.Response<string>("You are not authorized to access this resource", (int)code));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
        }

        public static void AddCustomOptions(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions<ExternalApiOptions>().BindConfiguration("ExternalApiOptions");
            serviceCollection.AddOptions<JWTSettings>().BindConfiguration("JWTSettings");
            serviceCollection.AddOptions<AdminOptions>().BindConfiguration("AdminOptions");
            serviceCollection.AddOptions<DatabaseOptions>().BindConfiguration("DatabaseOptions");
        }

        public static void AddCustomHostedServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<EmailBackgroundService>();
        }

        public static void AddCustomAutoMapper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(typeof(MappingProfileConfiguration));
        }

        public static void AddValidation(this IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = false; });
        }

        public static void AddSwaggerOpenAPI(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(setupAction =>
            {

                setupAction.SwaggerDoc(
                    "OpenAPISpecification",
                    new OpenApiInfo()
                    {
                        Title = "Third Template WebAPI",
                        Version = "1",
                        Description = "API Details for DockerShadow Admin",
                        Contact = new OpenApiContact()
                        {
                            Email = "info@theDockerShadow.com",
                            Name = "The Third Template",
                            Url = new Uri(" https://accessbankplc.com")
                        },
                        License = new OpenApiLicense()
                        {
                            Name = "UNLICENSED"
                        }
                    });

                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = $"Input your Bearer token in this format - Bearer token to access this API",
                });
                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        }, new List<string>()
                    },
                });
            });
        }

        public static void AddCustomControllers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddEndpointsApiExplorer();

            serviceCollection.AddControllersWithViews()
                .AddNewtonsoftJson(ops =>
                {
                    ops.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    ops.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                });
            serviceCollection.AddRazorPages();

            serviceCollection.Configure<ApiBehaviorOptions>(apiBehaviorOptions =>
                apiBehaviorOptions.InvalidModelStateResponseFactory = actionContext =>
                {
                    var logger = actionContext.HttpContext.RequestServices.GetRequiredService<ILogger<BadRequestObjectResult>>();
                    IEnumerable<string> errorList = actionContext.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                    logger.LogError("Bad Request");
                    logger.LogError(string.Join(",", errorList));
                    return new BadRequestObjectResult(new Domain.Common.Response<IEnumerable<string>>("DockerShadow Validation Error", 400, errorList));
                });
        }

        public static void AddHTTPPolicies(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var policyConfigs = new HttpClientPolicyConfiguration();
            configuration.Bind("HttpClientPolicies", policyConfigs);

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(policyConfigs.RetryTimeoutInSeconds));

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(policyConfigs.RetryCount, _ => TimeSpan.FromMilliseconds(policyConfigs.RetryDelayInMs));

            var circuitBreakerPolicy = HttpPolicyExtensions
               .HandleTransientHttpError()
               .CircuitBreakerAsync(policyConfigs.MaxAttemptBeforeBreak, TimeSpan.FromSeconds(policyConfigs.BreakDurationInSeconds));

            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

            HttpClientHandler handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            //Register a Typed Instance of HttpClientFactory for a Protected Resource
            //More info see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.0

            serviceCollection.AddHttpClient<IClientFactory, ClientFactory>()
                .ConfigurePrimaryHttpMessageHandler(_ =>
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                    };
                    return handler;
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(policyConfigs.HandlerTimeoutInMinutes))
                .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
                .AddPolicyHandler(timeoutPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);
        }

        public static void AddRequestRateLimiter(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // needed to load configuration from appsettings.json
            serviceCollection.AddOptions();
            // needed to store rate limit counters and ip rules
            serviceCollection.AddMemoryCache();

            //load general configuration from appsettings.json
            serviceCollection.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            // inject counter and rules stores
            serviceCollection.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            serviceCollection.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // configuration (resolvers, counter key builders)
            serviceCollection.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
