using DTOs.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Extensions;

/// <summary>
/// Extension methods for consistent API responses in controllers
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Returns 200 OK with wrapped data
    /// </summary>
    public static IActionResult ApiOk<T>(this ControllerBase controller, T data, string? message = null)
        => controller.Ok(ApiResponse<T>.Ok(data, message));

    /// <summary>
    /// Returns 200 OK without data payload
    /// </summary>
    public static IActionResult ApiOk(this ControllerBase controller, string? message = null)
        => controller.Ok(ApiResponse.Ok(message));

    /// <summary>
    /// Returns 404 Not Found with error message
    /// </summary>
    public static IActionResult ApiNotFound(this ControllerBase controller, string message)
        => controller.NotFound(ApiResponse.Fail(message, "NOT_FOUND"));

    /// <summary>
    /// Returns 400 Bad Request with error message
    /// </summary>
    public static IActionResult ApiBadRequest(this ControllerBase controller, string message)
        => controller.BadRequest(ApiResponse.Fail(message, "BAD_REQUEST"));

    /// <summary>
    /// Returns 400 Bad Request with validation errors from ModelState
    /// </summary>
    public static IActionResult ApiBadRequest(this ControllerBase controller, ModelStateDictionary modelState)
    {
        var errors = modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
        return controller.BadRequest(ApiResponse.Fail(errors, "VALIDATION_ERROR"));
    }

    /// <summary>
    /// Returns 201 Created with wrapped data
    /// </summary>
    public static IActionResult ApiCreated<T>(this ControllerBase controller, string actionName, object routeValues, T data)
        => controller.CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data));

    /// <summary>
    /// Returns 201 Created with wrapped data (StatusCode variant)
    /// </summary>
    public static IActionResult ApiCreated<T>(this ControllerBase controller, T data)
        => controller.StatusCode(StatusCodes.Status201Created, ApiResponse<T>.Ok(data));

    /// <summary>
    /// Returns 401 Unauthorized with error message
    /// </summary>
    public static IActionResult ApiUnauthorized(this ControllerBase controller, string message)
        => controller.Unauthorized(ApiResponse.Fail(message, "UNAUTHORIZED"));
}
