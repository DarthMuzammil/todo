using Microsoft.AspNetCore.Identity;
using Todo.Application.Auth;
using Todo.Application.Common;
using Todo.Application.Identity;
using Todo.Application.Interfaces;

namespace Todo.Application.Commands.Login;

public class LoginHandler
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthSessionResult>> HandleAsync(LoginCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Result<AuthSessionResult>.Failure(InvalidCredentialsMessage);

        var valid = await _userManager.CheckPasswordAsync(user, command.Password);
        if (!valid)
            return Result<AuthSessionResult>.Failure(InvalidCredentialsMessage);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.CreateAccessToken(user.Id, user.Email!, roles);
        var refreshToken = await _tokenService.IssueRefreshTokenAsync(user.Id);

        return Result<AuthSessionResult>.Success(
            new AuthSessionResult(
                new AuthResponse(accessToken, user.Id, user.Email!, user.Name),
                refreshToken.Token,
                refreshToken.ExpiresAt));
    }
}
