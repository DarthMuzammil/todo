using Todo.Application.Auth;
using Todo.Application.Common;
using Todo.Application.Interfaces;

namespace Todo.Application.Commands.RefreshToken;

public class RefreshTokenHandler
{
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<Result<RefreshSessionResult>> HandleAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result<RefreshSessionResult>.Failure("Refresh token is required.");

        var rotated = await _tokenService.RotateRefreshTokenAsync(refreshToken);
        if (rotated is null)
            return Result<RefreshSessionResult>.Failure("Invalid or expired refresh token.");

        return Result<RefreshSessionResult>.Success(
            new RefreshSessionResult(
                rotated.AccessToken,
                rotated.RefreshToken,
                rotated.RefreshTokenExpiresAt));
    }
}
