using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace Domain.Services;

/// <summary>
/// Service for managing refresh tokens (D9) and session management (D12)
/// Uses in-memory cache - tokens are lost on app restart
/// </summary>
public class RefreshTokenService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7);

    public RefreshTokenService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Generates a new refresh token for a user
    /// </summary>
    public string GenerateRefreshToken(string username)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var cacheKey = GetTokenKey(token);
        
        // Store token -> username mapping
        _cache.Set(cacheKey, username, _refreshTokenLifetime);
        
        // Store username -> tokens list for session management
        var userTokensKey = GetUserTokensKey(username);
        var tokens = _cache.GetOrCreate(userTokensKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _refreshTokenLifetime;
            return new HashSet<string>();
        });
        tokens!.Add(token);
        _cache.Set(userTokensKey, tokens, _refreshTokenLifetime);

        return token;
    }

    /// <summary>
    /// Validates a refresh token and returns the associated username
    /// </summary>
    public string? ValidateRefreshToken(string token)
    {
        var cacheKey = GetTokenKey(token);
        if (_cache.TryGetValue(cacheKey, out string? username))
        {
            return username;
        }
        return null;
    }

    /// <summary>
    /// Revokes a specific refresh token
    /// </summary>
    public void RevokeToken(string token)
    {
        var cacheKey = GetTokenKey(token);
        if (_cache.TryGetValue(cacheKey, out string? username))
        {
            _cache.Remove(cacheKey);
            
            // Remove from user's token list
            var userTokensKey = GetUserTokensKey(username!);
            if (_cache.TryGetValue(userTokensKey, out HashSet<string>? tokens))
            {
                tokens?.Remove(token);
            }
        }
    }

    /// <summary>
    /// Revokes all refresh tokens for a user (logout from all devices)
    /// </summary>
    public void RevokeAllUserTokens(string username)
    {
        var userTokensKey = GetUserTokensKey(username);
        if (_cache.TryGetValue(userTokensKey, out HashSet<string>? tokens))
        {
            foreach (var token in tokens ?? Enumerable.Empty<string>())
            {
                _cache.Remove(GetTokenKey(token));
            }
            _cache.Remove(userTokensKey);
        }
    }

    /// <summary>
    /// Gets the count of active sessions for a user
    /// </summary>
    public int GetActiveSessionCount(string username)
    {
        var userTokensKey = GetUserTokensKey(username);
        if (_cache.TryGetValue(userTokensKey, out HashSet<string>? tokens))
        {
            return tokens?.Count ?? 0;
        }
        return 0;
    }

    private static string GetTokenKey(string token) => $"refresh_token:{token}";
    private static string GetUserTokensKey(string username) => $"user_tokens:{username.ToLowerInvariant()}";
}
