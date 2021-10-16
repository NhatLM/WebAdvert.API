
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebAdvert.API.Services;

namespace WebAdvert.API.HealthChecks
{
    public class StorageHealthcheck : IHealthCheck
    {
        private readonly IAdvertStorageService _storageService;

        public StorageHealthcheck(IAdvertStorageService advertStorageService)
        {
            _storageService = advertStorageService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isStorageOk = await _storageService.CheckHealth();

            if (isStorageOk)
            {
                return HealthCheckResult.Healthy("A healthy result.");
            }

            return HealthCheckResult.Unhealthy("An unhealthy result.");
        }
    }
}
