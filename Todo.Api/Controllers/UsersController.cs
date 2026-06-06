using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Extensions;
using Todo.Application.Auth;
using Todo.Application.Commands.ChangePassword;
using Todo.Application.Commands.LogoutAllSessions;
using Todo.Application.Commands.UpdateProfile;
using Todo.Application.Identity;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UpdateProfileHandler _updateProfileHandler;
    private readonly ChangePasswordHandler _changePasswordHandler;
    private readonly LogoutAllSessionsHandler _logoutAllSessionsHandler;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        UpdateProfileHandler updateProfileHandler,
        ChangePasswordHandler changePasswordHandler,
        LogoutAllSessionsHandler logoutAllSessionsHandler)
    {
        _userManager = userManager;
        _updateProfileHandler = updateProfileHandler;
        _changePasswordHandler = changePasswordHandler;
        _logoutAllSessionsHandler = logoutAllSessionsHandler;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return NotFound(new { error = "User not found." });

        return Ok(new UserProfileResponse(user.Id, user.Email!, user.Name));
    }

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var result = await _updateProfileHandler.HandleAsync(
            new UpdateProfileCommand(User.GetUserId(), request.Name));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _changePasswordHandler.HandleAsync(
            new ChangePasswordCommand(
                User.GetUserId(),
                request.CurrentPassword,
                request.NewPassword));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    [HttpPost("me/logout-all")]
    public async Task<IActionResult> LogoutAllSessions()
    {
        var result = await _logoutAllSessionsHandler.HandleAsync(
            new LogoutAllSessionsCommand(User.GetUserId()));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
}
