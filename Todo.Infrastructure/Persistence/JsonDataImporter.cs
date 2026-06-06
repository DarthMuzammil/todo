using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Persistence;

public class JsonDataImporter
{
    private const string DefaultListColor = "#3B82F6";

    private readonly TodoDbContext _context;
    private readonly JsonDataPaths _paths;
    private readonly ILogger<JsonDataImporter> _logger;

    public JsonDataImporter(
        TodoDbContext context,
        JsonDataPaths paths,
        ILogger<JsonDataImporter> logger)
    {
        _context = context;
        _paths = paths;
        _logger = logger;
    }

    public async Task<JsonImportResult> ImportAsync(CancellationToken cancellationToken = default)
    {
        var lists = await LoadJsonFileAsync<TodoList>(_paths.ListsFile, cancellationToken);
        var tasks = await LoadJsonFileAsync<TodoTask>(_paths.TasksFile, cancellationToken);

        var usersImported = await ImportUsersAsync(lists, tasks, cancellationToken);
        var listsImported = await ImportListsAsync(lists, cancellationToken);
        var (tasksImported, tasksSkipped) = await ImportTasksAsync(tasks, cancellationToken);

        return new JsonImportResult(usersImported, listsImported, tasksImported, tasksSkipped);
    }

    private async Task<List<T>> LoadJsonFileAsync<T>(string filePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("JSON file not found, skipping import: {FilePath}", filePath);
            return [];
        }

        await using var stream = File.OpenRead(filePath);
        var items = await JsonSerializer.DeserializeAsync<List<T>>(stream, cancellationToken: cancellationToken);
        return items ?? [];
    }

    private async Task<int> ImportUsersAsync(
        IReadOnlyList<TodoList> lists,
        IReadOnlyList<TodoTask> tasks,
        CancellationToken cancellationToken)
    {
        var userIds = lists
            .Select(list => list.OwnerId)
            .Concat(tasks.Where(task => task.AssigneeId.HasValue).Select(task => task.AssigneeId!.Value))
            .Distinct()
            .ToList();

        var existingUserIds = await _context.Users
            .Where(user => userIds.Contains(user.Id))
            .Select(user => user.Id)
            .ToHashSetAsync(cancellationToken);

        var imported = 0;
        foreach (var userId in userIds)
        {
            if (existingUserIds.Contains(userId))
            {
                continue;
            }

            _context.Users.Add(CreatePlaceholderUser(userId));
            imported++;
        }

        if (imported > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return imported;
    }

    private async Task<int> ImportListsAsync(
        IReadOnlyList<TodoList> lists,
        CancellationToken cancellationToken)
    {
        var existingListIds = await _context.Lists
            .IgnoreQueryFilters()
            .Select(list => list.Id)
            .ToHashSetAsync(cancellationToken);

        var imported = 0;
        foreach (var list in lists)
        {
            if (existingListIds.Contains(list.Id))
            {
                continue;
            }

            list.Color = NormalizeColor(list.Color);
            _context.Lists.Add(list);
            imported++;
        }

        if (imported > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return imported;
    }

    private async Task<(int Imported, int Skipped)> ImportTasksAsync(
        IReadOnlyList<TodoTask> tasks,
        CancellationToken cancellationToken)
    {
        var validListIds = await _context.Lists
            .IgnoreQueryFilters()
            .Select(list => list.Id)
            .ToHashSetAsync(cancellationToken);

        var existingTaskIds = await _context.Tasks
            .IgnoreQueryFilters()
            .Select(task => task.Id)
            .ToHashSetAsync(cancellationToken);

        var imported = 0;
        var skipped = 0;

        foreach (var task in tasks)
        {
            if (existingTaskIds.Contains(task.Id))
            {
                continue;
            }

            if (!validListIds.Contains(task.ListId))
            {
                _logger.LogWarning(
                    "Skipping task {TaskId}: list {ListId} not found",
                    task.Id,
                    task.ListId);
                skipped++;
                continue;
            }

            if (task.AssigneeId.HasValue
                && !await _context.Users.AnyAsync(user => user.Id == task.AssigneeId, cancellationToken))
            {
                _context.Users.Add(CreatePlaceholderUser(task.AssigneeId.Value));
                await _context.SaveChangesAsync(cancellationToken);
            }

            _context.Tasks.Add(task);
            imported++;
        }

        if (imported > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return (imported, skipped);
    }

    private static User CreatePlaceholderUser(Guid id) =>
        new()
        {
            Id = id,
            Name = "Imported User",
            Email = $"import-{id:N}@todo.local"
        };

    private static string NormalizeColor(string? color) =>
        string.IsNullOrWhiteSpace(color) ? DefaultListColor : color;
}
