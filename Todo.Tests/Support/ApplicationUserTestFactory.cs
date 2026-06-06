using Todo.Application.Identity;
using Todo.Infrastructure.Persistence;

namespace Todo.Tests.Support;

public static class ApplicationUserTestFactory
{
    public static ApplicationUser Create(Guid? id = null, string name = "Test User")
    {
        var userId = id ?? Guid.NewGuid();
        var email = $"test-{userId:N}@local.test";

        return new ApplicationUser
        {
            Id = userId,
            Name = name,
            Email = email,
            UserName = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant()
        };
    }

    public static async Task<Guid> SeedAsync(TodoDbContext context, Guid? id = null, string name = "Test User")
    {
        var user = Create(id, name);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.Id;
    }
}
