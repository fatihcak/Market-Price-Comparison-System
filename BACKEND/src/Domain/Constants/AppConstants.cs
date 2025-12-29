namespace Domain.Constants;

/// <summary>
/// Application-wide constants to avoid magic numbers
/// </summary>
public static class AppConstants
{
    /// <summary>
    /// Pagination defaults
    /// </summary>
    public static class Pagination
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;
        public const int MinPageSize = 1;
    }

    /// <summary>
    /// Search related constants
    /// </summary>
    public static class Search
    {
        public const int MaxLevenshteinDistance = 3;
        public const int DefaultSearchLimit = 50;
        public const int FuzzySearchLimit = 10;
        public const int QuickSearchLimit = 5;
    }

    /// <summary>
    /// Chat/AI related constants
    /// </summary>
    public static class Chat
    {
        public const int MaxProductsInResponse = 5;
        public const int MaxAlternativeMarkets = 3;
    }

    /// <summary>
    /// Cache durations in minutes
    /// </summary>
    public static class Cache
    {
        public const int ShortDuration = 5;
        public const int MediumDuration = 30;
        public const int LongDuration = 60;
    }
}
