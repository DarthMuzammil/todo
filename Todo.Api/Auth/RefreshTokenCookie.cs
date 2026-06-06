using Microsoft.AspNetCore.Http;

namespace Todo.Api.Auth;

public static class RefreshTokenCookie
{
    public const string Name = "todo.refreshToken";
    public const string Path = "/api/auth";

    public static CookieOptions CreateOptions(IHostEnvironment environment, DateTime expiresAt)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !environment.IsDevelopment(),
            SameSite = environment.IsDevelopment()
                ? SameSiteMode.Lax
                : SameSiteMode.None,
            Path = Path,
            Expires = expiresAt,
        };
    }

    public static void Append(HttpResponse response, IHostEnvironment environment, string token, DateTime expiresAt)
    {
        response.Cookies.Append(Name, token, CreateOptions(environment, expiresAt));
    }

    public static void Clear(HttpResponse response, IHostEnvironment environment)
    {
        response.Cookies.Delete(Name, CreateOptions(environment, DateTime.UtcNow.AddDays(-1)));
    }
}
