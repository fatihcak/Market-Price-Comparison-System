using Microsoft.Extensions.Hosting;

namespace API.Services;

public class CacheRefreshJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CacheRefreshJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Wait for 6 hours
            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);

            try
            {
                // Create a scope to resolve scoped services (ICacheWarmer / IProductService)
                using (var scope = _serviceProvider.CreateScope())
                {
                    var warmer = scope.ServiceProvider.GetRequiredService<ICacheWarmer>();
                    await warmer.WarmupAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BackgroundJob] Cache refresh failed: {ex.Message}");
            }
        }
    }
}
