using Microsoft.AspNetCore.Mvc;
using Postech.Hackathon.GestorCadastro.Api.Models;

namespace Postech.Hackathon.GestorCadastro.Api.Settings;

public static class ControllerBaseExtensions
{
    public static IActionResult ApiBadRequest(this ControllerBase controller, string message, string? details = null)
    {
        var response = new ApiErrorResponse(StatusCodes.Status400BadRequest, message, details);
        return controller.BadRequest(response);
    }

    public static IActionResult ApiUnauthorized(this ControllerBase controller, string message, string? details = null)
    {
        var response = new ApiErrorResponse(StatusCodes.Status401Unauthorized, message, details);
        return controller.StatusCode(StatusCodes.Status401Unauthorized, response);
    }

    public static IActionResult ApiForbidden(this ControllerBase controller, string message, string? details = null)
    {
        var response = new ApiErrorResponse(StatusCodes.Status403Forbidden, message, details);
        return controller.StatusCode(StatusCodes.Status403Forbidden, response);
    }

    public static IActionResult ApiNotFound(this ControllerBase controller, string message, string? details = null)
    {
        var response = new ApiErrorResponse(StatusCodes.Status404NotFound, message, details);
        return controller.NotFound(response);
    }

    public static IActionResult ApiInternalServerError(this ControllerBase controller, string message, string? details = null)
    {
        var response = new ApiErrorResponse(StatusCodes.Status500InternalServerError, message, details);
        return controller.StatusCode(StatusCodes.Status500InternalServerError, response);
    }
} 