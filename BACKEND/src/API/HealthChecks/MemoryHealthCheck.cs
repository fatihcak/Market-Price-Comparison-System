using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace API.HealthChecks;

/// <summary>
/// Health check for memory usage monitoring
/// </summary>
public class MemoryHealthCheck : IHealthCheck
{
    private readonly long _degradedThresholdBytes;
    private readonly long _unhealthyThresholdBytes;

    public MemoryHealthCheck()
    {
        // Default thresholds: 500MB degraded, 1GB unhealthy
        _degradedThresholdBytes = 500 * 1024 * 1024;
        _unhealthyThresholdBytes = 1024 * 1024 * 1024;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var allocatedBytes = GC.GetTotalMemory(forceFullCollection: false);
        var data = new Dictionary<string, object>
        {
            { "AllocatedBytes", allocatedBytes },
            { "AllocatedMB", allocatedBytes / (1024 * 1024) },
            { "Gen0Collections", GC.CollectionCount(0) },
            { "Gen1Collections", GC.CollectionCount(1) },
            { "Gen2Collections", GC.CollectionCount(2) }
        };

        if (allocatedBytes >= _unhealthyThresholdBytes)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                $"Memory usage is critical: {allocatedBytes / (1024 * 1024)}MB",
                data: data));
        }

        if (allocatedBytes >= _degradedThresholdBytes)
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                $"Memory usage is high: {allocatedBytes / (1024 * 1024)}MB",
                data: data));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            $"Memory usage is normal: {allocatedBytes / (1024 * 1024)}MB",
            data: data));
    }
}
