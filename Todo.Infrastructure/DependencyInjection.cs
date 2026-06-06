using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Identity;
using Todo.Application.Interfaces;
using Todo.Infrastructure.Identity;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Repositories;

namespace Todo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string tasksFilePath,
        string listsFilePath)
    {
        var connectionString = ResolveSqliteConnectionString(configuration);

        services.AddDbContext<TodoDbContext>(options =>
            options.UseSqlite(connectionString));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<TodoDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<JsonDataImporter>();

        services.AddSingleton(new JsonDataPaths(
            Path.GetFullPath(listsFilePath),
            Path.GetFullPath(tasksFilePath)));

        services.AddScoped<IListRepository, EfListRepository>();
        services.AddScoped<ITaskRepository, EfTaskRepository>();
        services.AddScoped<IWorkspaceRepository, EfWorkspaceRepository>();
        services.AddScoped<IWorkspaceInviteRepository, EfWorkspaceInviteRepository>();
        services.AddScoped<IInviteTokenService, InviteTokenService>();
        services.AddScoped<IActivityRepository, EfActivityRepository>();
        services.AddScoped<INotificationRepository, EfNotificationRepository>();
        services.AddScoped<IUserDirectory, EfUserDirectory>();
        services.AddScoped<WorkspaceDataBackfill>();

        return services;
    }

    private static string ResolveSqliteConnectionString(IConfiguration configuration)
    {
        var configured = configuration.GetConnectionString("Default")
            ?? "Data Source=data/todo.db";

        const string prefix = "Data Source=";
        if (!configured.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return configured;
        }

        var relativePath = configured[prefix.Length..];
        if (Path.IsPathRooted(relativePath))
        {
            return configured;
        }

        var absolutePath = Path.GetFullPath(relativePath);
        var directory = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return $"{prefix}{absolutePath}";
    }
}
