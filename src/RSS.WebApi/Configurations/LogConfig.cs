using HealthChecks.UI.Client;
using KissLog;
using KissLog.AspNetCore;
using KissLog.CloudListeners.Auth;
using KissLog.CloudListeners.RequestLogsListener;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RSS.WebApi.Extensions;
using System;
using System.Diagnostics;
using System.Text;

namespace RSS.WebApi.Configurations
{
    public static class LogConfig
    {
        public static IServiceCollection AddLogConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks().AddHealthChecksConfiguration(configuration);

            services.AddHealthChecksUI(setup => {
                var healthCheckSettings = configuration.GetSection("HealthChecks-UI:HealthChecks").Get<HealthCheckSettings>();
                setup.AddHealthCheckEndpoint(healthCheckSettings.Name, healthCheckSettings.Uri);
            })
            //Salva logs do HealthCheck no banco, mas podemos usar em memoria
            .AddSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));

            //adicionando KissLog para logs
            services.AddScoped(context => Logger.Factory.Get());

            services.AddLogging(logging =>
            {
                logging.AddKissLog();
            });

            return services;
        }

        public static IApplicationBuilder UseLogConfiguration(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env)
        {
           
            app.UseKissLogMiddleware(options => {
                LogConfig.ConfigureKissLog(options, configuration);
            });

            app.UseHealthChecks("/api/hc", new HealthCheckOptions { 
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI(options => {
                options.UIPath = "/api/hc-ui";

                options.UseRelativeApiPath = false;
                options.UseRelativeResourcesPath = false;
                options.UseRelativeWebhookPath = false;
            });

            return app;
        }

        public static void RegisterKissLogListeners(IConfiguration configuration, IOptionsBuilder options)
        {
            options.Listeners.Add(new RequestLogsApiListener(new Application(
                configuration["KissLog.OrganizationId"],
                configuration["KissLog.ApplicationId"])
            )
            {
                ApiUrl = configuration["KissLog.ApiUrl"]
            });
        }

        public static void ConfigureKissLog(IOptionsBuilder options, IConfiguration configuration)
        {
            // optional KissLog configuration
            options.Options
                .AppendExceptionDetails((Exception ex) =>
                {
                    StringBuilder sb = new StringBuilder();

                    if (ex is System.NullReferenceException nullRefException)
                    {
                        sb.AppendLine("Important: check for null references");
                    }

                    return sb.ToString();
                });

            // KissLog internal logs
            options.InternalLog = (message) =>
            {
                Debug.WriteLine(message);
            };

            RegisterKissLogListeners(configuration, options);
        }
    }
}
