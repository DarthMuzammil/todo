using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Todo.Application.Identity;
using Todo.Application.Interfaces;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly TodoDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenService(
        IOptions<JwtSettings> options,
        TodoDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _settings = options.Value;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public string CreateAccessToken(Guid userId, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<IssuedRefreshToken> IssueRefreshTokenAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var plainToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = HashToken(plainToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new IssuedRefreshToken(plainToken, expiresAt);
    }

    public async Task<RotatedTokens?> RotateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _dbContext.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            return null;

        var user = storedToken.User;
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = CreateAccessToken(user.Id, user.Email!, roles);

        var plainToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);
        var replacementId = Guid.NewGuid();

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByTokenId = replacementId;

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = replacementId,
            UserId = user.Id,
            TokenHash = HashToken(plainToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RotatedTokens(accessToken, plainToken, expiresAt);
    }

    public async Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || storedToken.RevokedAt is not null)
            return;

        storedToken.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var activeTokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        if (activeTokens.Count == 0)
            return;

        var now = DateTime.UtcNow;
        foreach (var token in activeTokens)
            token.RevokedAt = now;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
