using Todo.Application.Common;
using Todo.Application.Interfaces;

namespace Todo.Application.Commands.LogoutAllSessions;

public class LogoutAllSessionsHandler
{
    private readonly ITokenService _tokenService;

    public LogoutAllSessionsHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<Result> HandleAsync(LogoutAllSessionsCommand command)
    {
        await _tokenService.RevokeAllRefreshTokensAsync(command.UserId);
        return Result.Success();
    }
}
