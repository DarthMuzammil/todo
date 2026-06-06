using Microsoft.AspNetCore.Mvc;
using Todo.Application.Common;

namespace Todo.Api.Extensions;

public static class ListAccessResultExtensions
{
    public static IActionResult ToListReadResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Success results should not use ToListReadResult.");

        return MapReadError(controller, result.Error);
    }

    public static IActionResult ToListMutationResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Success results should not use ToListMutationResult.");

        return MapMutationError(controller, result.Error);
    }

    private static IActionResult MapReadError(ControllerBase controller, string? error)
    {
        return error == ListAccessChecker.NotFoundMessage
            ? controller.NotFound(new { error })
            : controller.BadRequest(new { error });
    }

    private static IActionResult MapMutationError(ControllerBase controller, string? error)
    {
        return error switch
        {
            ListAccessChecker.NotFoundMessage => controller.NotFound(new { error }),
            ListAccessChecker.ForbiddenMessage => controller.StatusCode(
                StatusCodes.Status403Forbidden,
                new { error }),
            _ => controller.BadRequest(new { error }),
        };
    }
}
