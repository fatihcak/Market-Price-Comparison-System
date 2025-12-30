namespace DTOs.DTOs.Common;

/// <summary>
/// Standard API response wrapper for consistent response format
/// </summary>
/// <typeparam name="T">Type of the data payload</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public string? Code { get; set; }

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    /// <summary>
    /// Creates a failed response with a single error
    /// </summary>
    public static ApiResponse<T> Fail(string error, string? code = null)
        => new() { Success = false, Errors = new List<string> { error }, Code = code };

    /// <summary>
    /// Creates a failed response with multiple errors
    /// </summary>
    public static ApiResponse<T> Fail(List<string> errors, string? code = null)
        => new() { Success = false, Errors = errors, Code = code };
}

/// <summary>
/// Non-generic API response for operations without data payload
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Creates a successful response without data
    /// </summary>
    public static ApiResponse Ok(string? message = null)
        => new() { Success = true, Message = message };

    /// <summary>
    /// Creates a failed response with a single error
    /// </summary>
    public new static ApiResponse Fail(string error, string? code = null)
        => new() { Success = false, Errors = new List<string> { error }, Code = code };

    /// <summary>
    /// Creates a failed response with multiple errors
    /// </summary>
    public new static ApiResponse Fail(List<string> errors, string? code = null)
        => new() { Success = false, Errors = errors, Code = code };
}
