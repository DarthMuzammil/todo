using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Interfaces;
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

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<JsonDataImporter>();

        services.AddSingleton(new JsonDataPaths(
            Path.GetFullPath(listsFilePath),
            Path.GetFullPath(tasksFilePath)));

        // services.AddSingleton<ITaskRepository>(_ => new JsonTaskRepository(tasksFilePath));
        // services.AddSingleton<IListRepository>(_ => new JsonListRepository(listsFilePath));
        services.AddScoped<IListRepository, EfListRepository>();
        services.AddScoped<ITaskRepository, EfTaskRepository>();

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
