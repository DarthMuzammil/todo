namespace Todo.Application.Interfaces;

public interface IInviteTokenService
{
    string GenerateToken();
    string HashToken(string token);
}
