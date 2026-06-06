namespace Todo.Application.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(Guid userId, string email, IEnumerable<string> roles);

    Task<IssuedRefreshToken> IssueRefreshTokenAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<RotatedTokens?> RotateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task RevokeAllRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

public record IssuedRefreshToken(string Token, DateTime ExpiresAt);

public record RotatedTokens(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);
