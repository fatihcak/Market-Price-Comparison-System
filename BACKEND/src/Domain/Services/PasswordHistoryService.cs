using Microsoft.Extensions.Caching.Memory;

namespace Domain.Services;

/// <summary>
/// Service for tracking password history to prevent reuse (D11)
/// Uses in-memory cache - history is lost on app restart
/// </summary>
public class PasswordHistoryService
{
    private readonly IMemoryCache _cache;
    private readonly int _historyCount = 5; // Keep last 5 passwords

    public PasswordHistoryService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Checks if password was recently used
    /// </summary>
    public bool IsPasswordReused(string username, string newPassword)
    {
        var cacheKey = GetHistoryKey(username);
        if (_cache.TryGetValue(cacheKey, out List<string>? passwordHashes))
        {
            foreach (var hash in passwordHashes ?? Enumerable.Empty<string>())
            {
                if (BCrypt.Net.BCrypt.Verify(newPassword, hash))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Adds password to history after successful change
    /// </summary>
    public void AddPasswordToHistory(string username, string passwordHash)
    {
        var cacheKey = GetHistoryKey(username);
        var history = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromDays(365); // Keep for 1 year
            return new List<string>();
        });

        history!.Add(passwordHash);
        
        // Keep only last N passwords
        while (history.Count > _historyCount)
        {
            history.RemoveAt(0);
        }

        _cache.Set(cacheKey, history, TimeSpan.FromDays(365));
    }

    private static string GetHistoryKey(string username) => $"password_history:{username.ToLowerInvariant()}";
}
