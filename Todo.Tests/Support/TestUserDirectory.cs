using Todo.Application.Interfaces;

namespace Todo.Tests.Support;

public sealed class TestUserDirectory : IUserDirectory
{
    public Task<string> GetDisplayNameAsync(Guid userId) =>
        Task.FromResult("Test User");
}
