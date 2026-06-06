using Microsoft.AspNetCore.Identity;
using Todo.Application.Auth;
using Todo.Application.Common;
using Todo.Application.Identity;
using Todo.Application.Interfaces;

namespace Todo.Application.Commands.Register;

public class RegisterHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PersonalWorkspaceService _personalWorkspaceService;

    public RegisterHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        PersonalWorkspaceService personalWorkspaceService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _personalWorkspaceService = personalWorkspaceService;
    }

    public async Task<Result<AuthSessionResult>> HandleAsync(RegisterCommand command)
    {
        var user = new ApplicationUser
        {
            UserName = command.Email,
            Email = command.Email,
            Name = command.Name
        };

        var result = await _userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            var error = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<AuthSessionResult>.Failure(error);
        }

        var workspaceResult = await _personalWorkspaceService.EnsurePersonalWorkspaceAsync(user.Id);
        if (!workspaceResult.IsSuccess)
            return Result<AuthSessionResult>.Failure(workspaceResult.Error!);

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
