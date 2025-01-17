using DockerShadow.Domain.Settings;
using DockerShadow.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DockerShadow.Infrastructure.Extension
{
    public static class ConfigureContainer
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }

        public static void ConfigureSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(setupAction =>
                {
                    setupAction.SwaggerEndpoint("/swagger/OpenAPISpecification/swagger.json", "DockerShadow Admin API");
                    setupAction.RoutePrefix = "swagger";
                });
            }
        }

        public static void ConfigureCors(this IApplicationBuilder app, IConfiguration configuration)
        {
            AdminOptions adminOptions = new AdminOptions();
            configuration.Bind("AdminOptions", adminOptions);

            app.UseCors(options =>
                 options.WithOrigins(adminOptions.AllowedHosts)
                 .AllowAnyHeader()
                 .WithExposedHeaders("Content-Disposition")
                 .AllowAnyMethod());
        }
    }
}
