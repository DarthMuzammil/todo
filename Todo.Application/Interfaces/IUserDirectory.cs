namespace Todo.Application.Interfaces;

public interface IUserDirectory
{
    Task<string> GetDisplayNameAsync(Guid userId);
}
