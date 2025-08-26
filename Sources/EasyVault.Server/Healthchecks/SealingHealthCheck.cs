using EasyVault.Server.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EasyVault.Server.Healthchecks
{
    public class SealingHealthCheck(IVault vault) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(vault.IsSealed
                ? HealthCheckResult.Unhealthy("The vault is sealed.")
                : HealthCheckResult.Healthy("The vault is not sealed."));
        }
    }
}
