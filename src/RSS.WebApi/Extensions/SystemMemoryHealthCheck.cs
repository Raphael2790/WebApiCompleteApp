using KissLog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RSS.WebApi.Extensions
{
    public class SystemMemoryHealthCheck : IHealthCheck
    {
        private readonly ILogger _logger;
        public SystemMemoryHealthCheck(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var client = new MemoryMetricsClient();
            var metrics = client.GetMetrics();
            var percentUsed = 100 * metrics.Used / metrics.Total;
            var status = HealthStatus.Healthy;

            if (percentUsed > 80)
            {
                status = HealthStatus.Degraded;
            }

            if (percentUsed > 90)
            {
                status = HealthStatus.Unhealthy;
                _logger.Critical($"Uso de memória elevado, uso atual é de {percentUsed}%");
            }

            var data = new Dictionary<string, object>();
            data.Add("Total", metrics.Total);
            data.Add("Used", metrics.Used);
            data.Add("Free", metrics.Free);

            var result = new HealthCheckResult(status, null, null, data);
            return await Task.FromResult(result);
        }
    }
}
