using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RSS.WebApi.Extensions
{
    public class GarbageCollectorHealthCheck : IHealthCheck
    {
        private readonly IOptionsMonitor<GCInfoOptions> _options;

        public GarbageCollectorHealthCheck(IOptionsMonitor<GCInfoOptions> options)
        {
            _options = options;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = _options.Get(context.Registration.Name);

            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var data = new Dictionary<string, object>()
            {
                { "Allocated", allocated },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) },
            };

            var result = allocated >= options.Threshold ? context.Registration.FailureStatus : HealthStatus.Healthy;

            return Task.FromResult(new HealthCheckResult(
                result,
                description: "reporta status de degradação se alocado >= 1gb de memória",
                data: data));
        }

        public class GCInfoOptions
        {
            public long Threshold { get; set; } = 1024L * 1024L * 1024L;
        }
    }
}
