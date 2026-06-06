using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Todo.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TodoDbContext>>();

        await context.Database.MigrateAsync();
        var backfill = scope.ServiceProvider.GetRequiredService<WorkspaceDataBackfill>();
        await backfill.RunAsync();

        logger.LogInformation("SQLite database initialized at {Path}", GetDatabasePath(context));

        if (configuration.GetValue("Data:ImportOnStartup", true))
        {
            var importer = scope.ServiceProvider.GetRequiredService<JsonDataImporter>();
            var result = await importer.ImportAsync();

            if (result.UsersImported + result.ListsImported + result.TasksImported + result.TasksSkipped > 0)
            {
                logger.LogInformation(
                    "JSON import complete: {Users} users, {Lists} lists, {Tasks} tasks imported; {Skipped} tasks skipped",
                    result.UsersImported,
                    result.ListsImported,
                    result.TasksImported,
                    result.TasksSkipped);
            }
        }
    }

    private static string GetDatabasePath(TodoDbContext context)
    {
        var connectionString = context.Database.GetConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            return "unknown";
        }

        const string prefix = "Data Source=";
        return connectionString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? connectionString[prefix.Length..]
            : connectionString;
    }
}
