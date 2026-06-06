namespace Todo.Application.Auth;

public record RegisterRequest(string Email, string Password, string Name);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string AccessToken, Guid UserId, string Email, string Name);
public record UserProfileResponse(Guid Id, string Email, string Name);
public record UpdateProfileRequest(string Name);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record AuthSessionResult(
    AuthResponse Response,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);
public record RefreshResponse(string AccessToken);
public record RefreshSessionResult(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);
