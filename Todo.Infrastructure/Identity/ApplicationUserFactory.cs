using Todo.Application.Identity;

namespace Todo.Infrastructure.Identity;

public static class ApplicationUserFactory
{
    public static ApplicationUser CreatePlaceholder(Guid id, string name = "Imported User")
    {
        var email = $"import-{id:N}@todo.local";

        return new ApplicationUser
        {
            Id = id,
            Name = name,
            Email = email,
            UserName = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant()
        };
    }
}
