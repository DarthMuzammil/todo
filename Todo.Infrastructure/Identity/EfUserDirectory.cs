using Microsoft.AspNetCore.Identity;
using Todo.Application.Identity;
using Todo.Application.Interfaces;

namespace Todo.Infrastructure.Identity;

public class EfUserDirectory : IUserDirectory
{
    private readonly UserManager<ApplicationUser> _userManager;

    public EfUserDirectory(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GetDisplayNameAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return "Someone";

        return string.IsNullOrWhiteSpace(user.Name) ? user.Email ?? "Someone" : user.Name;
    }
}
