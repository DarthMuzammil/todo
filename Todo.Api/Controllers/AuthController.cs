using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Auth;
using Todo.Application.Auth;
using Todo.Application.Commands.Login;
using Todo.Application.Commands.Logout;
using Todo.Application.Commands.RefreshToken;
using Todo.Application.Commands.Register;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterHandler _registerHandler;
    private readonly LoginHandler _loginHandler;
    private readonly RefreshTokenHandler _refreshTokenHandler;
    private readonly LogoutHandler _logoutHandler;
    private readonly IHostEnvironment _environment;

    public AuthController(
        RegisterHandler registerHandler,
        LoginHandler loginHandler,
        RefreshTokenHandler refreshTokenHandler,
        LogoutHandler logoutHandler,
        IHostEnvironment environment)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshTokenHandler = refreshTokenHandler;
        _logoutHandler = logoutHandler;
        _environment = environment;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password)
            || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Email, password, and name are required." });
        }

        var result = await _registerHandler.HandleAsync(
            new RegisterCommand(request.Email, request.Password, request.Name));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return OkWithRefreshCookie(result.Value!);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email and password are required." });

        var result = await _loginHandler.HandleAsync(
            new LoginCommand(request.Email, request.Password));

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        return OkWithRefreshCookie(result.Value!);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookie.Name, out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new { error = "Refresh token is required." });
        }

        var result = await _refreshTokenHandler.HandleAsync(refreshToken);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        var session = result.Value!;
        RefreshTokenCookie.Append(
            Response,
            _environment,
            session.RefreshToken,
            session.RefreshTokenExpiresAt);

        return Ok(new RefreshResponse(session.AccessToken));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        Request.Cookies.TryGetValue(RefreshTokenCookie.Name, out var refreshToken);
        await _logoutHandler.HandleAsync(refreshToken);
        RefreshTokenCookie.Clear(Response, _environment);

        return NoContent();
    }

    private IActionResult OkWithRefreshCookie(AuthSessionResult session)
    {
        RefreshTokenCookie.Append(
            Response,
            _environment,
            session.RefreshToken,
            session.RefreshTokenExpiresAt);

        return Ok(session.Response);
    }
}
