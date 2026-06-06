using Microsoft.AspNetCore.Identity;
using Todo.Application.Common;
using Todo.Application.Identity;

namespace Todo.Application.Commands.ChangePassword;

public class ChangePasswordHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ChangePasswordHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> HandleAsync(ChangePasswordCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.CurrentPassword))
            return Result.Failure("Current password is required.");

        if (string.IsNullOrWhiteSpace(command.NewPassword))
            return Result.Failure("New password is required.");

        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Result.Failure("User not found.");

        var result = await _userManager.ChangePasswordAsync(
            user,
            command.CurrentPassword,
            command.NewPassword);

        if (!result.Succeeded)
        {
            var error = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(error);
        }

        return Result.Success();
    }
}
