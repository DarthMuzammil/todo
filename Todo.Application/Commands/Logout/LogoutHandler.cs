using Todo.Application.Common;
using Todo.Application.Interfaces;

namespace Todo.Application.Commands.Logout;

public class LogoutHandler
{
    private readonly ITokenService _tokenService;

    public LogoutHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<Result> HandleAsync(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Success();

        await _tokenService.RevokeRefreshTokenAsync(refreshToken);
        return Result.Success();
    }
}
