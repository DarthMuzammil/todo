using Microsoft.AspNetCore.Identity;

namespace Todo.Application.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
}
