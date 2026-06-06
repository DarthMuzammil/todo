using Microsoft.AspNetCore.Identity;
using Todo.Application.Auth;
using Todo.Application.Common;
using Todo.Application.Identity;

namespace Todo.Application.Commands.UpdateProfile;

public class UpdateProfileHandler
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateProfileHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserProfileResponse>> HandleAsync(UpdateProfileCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<UserProfileResponse>.Failure("Name is required.");

        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return Result<UserProfileResponse>.Failure("User not found.");

        user.Name = command.Name.Trim();
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var error = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<UserProfileResponse>.Failure(error);
        }

        return Result<UserProfileResponse>.Success(
            new UserProfileResponse(user.Id, user.Email!, user.Name));
    }
}
