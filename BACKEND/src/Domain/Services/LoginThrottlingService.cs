using Microsoft.Extensions.Caching.Memory;

namespace Domain.Services;

/// <summary>
/// Service for tracking and throttling failed login attempts
/// Provides brute-force attack protection
/// </summary>
public class LoginThrottlingService
{
    private readonly IMemoryCache _cache;
    private readonly int _maxAttempts;
    private readonly TimeSpan _lockoutDuration;
    private readonly TimeSpan _attemptWindow;

    public LoginThrottlingService(IMemoryCache cache)
    {
        _cache = cache;
        _maxAttempts = 5; // Maximum failed attempts before lockout
        _lockoutDuration = TimeSpan.FromMinutes(15); // Lockout duration
        _attemptWindow = TimeSpan.FromMinutes(5); // Time window for counting attempts
    }

    /// <summary>
    /// Checks if a user is currently locked out
    /// </summary>
    public bool IsLockedOut(string username)
    {
        var lockoutKey = GetLockoutKey(username);
        return _cache.TryGetValue(lockoutKey, out DateTime lockoutEnd) && lockoutEnd > DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the remaining lockout time if user is locked out
    /// </summary>
    public TimeSpan? GetRemainingLockoutTime(string username)
    {
        var lockoutKey = GetLockoutKey(username);
        if (_cache.TryGetValue(lockoutKey, out DateTime lockoutEnd) && lockoutEnd > DateTime.UtcNow)
        {
            return lockoutEnd - DateTime.UtcNow;
        }
        return null;
    }

    /// <summary>
    /// Records a failed login attempt
    /// </summary>
    public void RecordFailedAttempt(string username)
    {
        var attemptKey = GetAttemptKey(username);
        var attempts = _cache.GetOrCreate(attemptKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _attemptWindow;
            return 0;
        });

        attempts++;
        _cache.Set(attemptKey, attempts, _attemptWindow);

        if (attempts >= _maxAttempts)
        {
            // Lock the account
            var lockoutKey = GetLockoutKey(username);
            _cache.Set(lockoutKey, DateTime.UtcNow.Add(_lockoutDuration), _lockoutDuration);
            
            // Clear attempts
            _cache.Remove(attemptKey);
        }
    }

    /// <summary>
    /// Clears failed attempts on successful login
    /// </summary>
    public void ClearFailedAttempts(string username)
    {
        var attemptKey = GetAttemptKey(username);
        _cache.Remove(attemptKey);
    }

    /// <summary>
    /// Gets the current number of failed attempts
    /// </summary>
    public int GetFailedAttemptCount(string username)
    {
        var attemptKey = GetAttemptKey(username);
        return _cache.TryGetValue(attemptKey, out int attempts) ? attempts : 0;
    }

    private static string GetAttemptKey(string username) => $"login_attempts:{username.ToLowerInvariant()}";
    private static string GetLockoutKey(string username) => $"login_lockout:{username.ToLowerInvariant()}";
}
