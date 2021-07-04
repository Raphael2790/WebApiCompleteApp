using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RSS.WebApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RSS.WebApi.Extensions.GarbageCollectorHealthCheck;

namespace RSS.WebApi.Configurations
{
    public static class HealthCheckConfig
    {
        public static IHealthChecksBuilder AddHealthChecksConfiguration(this IHealthChecksBuilder builder,
                                                                        IConfiguration configuration,
                                                                        HealthStatus? failureStatus = null,
                                                                        IEnumerable<string> tags = null,
                                                                        long? thresholdInBytes = null)
        {

            builder.AddCheck<GarbageCollectorHealthCheck>("Garbage Collector", failureStatus ?? HealthStatus.Degraded, tags);
            builder.AddCheck("HasProductsInDatabase", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")));
            builder.AddCheck<SystemMemoryHealthCheck>("MemoryCheck");
            builder.AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL");

            if (thresholdInBytes.HasValue)
            {
                builder.Services.Configure<GCInfoOptions>("Garbage Collector", options =>
                {
                    options.Threshold = thresholdInBytes.Value;
                });
            }

            return builder;
        }
    }
}
