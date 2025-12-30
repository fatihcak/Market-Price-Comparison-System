using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace API.HealthChecks;

/// <summary>
/// Custom health check for external API dependencies (e.g., Google Gemini)
/// </summary>
public class ExternalApiHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public ExternalApiHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKey = _configuration["GoogleAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return Task.FromResult(HealthCheckResult.Degraded("Google AI API key is not configured"));
            }

            // Simple check - just verify the key exists and has reasonable length
            if (apiKey.Length < 20)
            {
                return Task.FromResult(HealthCheckResult.Degraded("Google AI API key appears to be invalid"));
            }

            return Task.FromResult(HealthCheckResult.Healthy("External API configuration is valid"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("External API check failed", ex));
        }
    }
}

