using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Infrastructure.Persistence;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Tests.Persistence;

[TestFixture]
public class JsonDataImporterTests
{
    private string _dbPath = null!;
    private string _listsFile = null!;
    private string _tasksFile = null!;
    private TodoDbContext _context = null!;
    private JsonDataImporter _importer = null!;

    [SetUp]
    public async Task SetUp()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"todo-import-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        _dbPath = Path.Combine(tempDir, "test.db");
        _listsFile = Path.Combine(tempDir, "lists.json");
        _tasksFile = Path.Combine(tempDir, "tasks.json");

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        _context = new TodoDbContext(options);
        await _context.Database.MigrateAsync();

        _importer = new JsonDataImporter(
            _context,
            new JsonDataPaths(_listsFile, _tasksFile),
            NullLogger<JsonDataImporter>.Instance);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        if (Directory.Exists(Path.GetDirectoryName(_dbPath)!))
        {
            Directory.Delete(Path.GetDirectoryName(_dbPath)!, recursive: true);
        }
    }

    [Test]
    public async Task ImportAsync_ImportsUsersListsAndTasks()
    {
        var ownerId = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        await WriteListsAsync([
            new TodoList
            {
                Id = listId,
                OwnerId = ownerId,
                Title = "Work",
                Color = null!,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        ]);

        await WriteTasksAsync([
            new TodoTask
            {
                Id = taskId,
                ListId = listId,
                Title = "Ship import",
                Description = "",
                Status = TaskStatus.Todo,
                Priority = Priority.Medium,
                SortOrder = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        ]);

        var result = await _importer.ImportAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result.UsersImported, Is.EqualTo(1));
            Assert.That(result.ListsImported, Is.EqualTo(1));
            Assert.That(result.TasksImported, Is.EqualTo(1));
            Assert.That(result.TasksSkipped, Is.EqualTo(0));
        });

        var list = await _context.Lists.IgnoreQueryFilters().SingleAsync();
        Assert.That(list.Color, Is.EqualTo("#3B82F6"));
    }

    [Test]
    public async Task ImportAsync_IsIdempotentOnSecondRun()
    {
        var ownerId = Guid.NewGuid();
        var listId = Guid.NewGuid();

        await WriteListsAsync([
            new TodoList
            {
                Id = listId,
                OwnerId = ownerId,
                Title = "Work",
                Color = "#ffffff",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        ]);

        await WriteTasksAsync([]);

        var first = await _importer.ImportAsync();
        var second = await _importer.ImportAsync();

        Assert.Multiple(() =>
        {
            Assert.That(first.ListsImported, Is.EqualTo(1));
            Assert.That(second.ListsImported, Is.EqualTo(0));
            Assert.That(second.UsersImported, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task ImportAsync_SkipsTasksWithMissingList()
    {
        var ownerId = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var missingListId = Guid.NewGuid();

        await WriteListsAsync([
            new TodoList
            {
                Id = listId,
                OwnerId = ownerId,
                Title = "Work",
                Color = "#ffffff",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        ]);

        await WriteTasksAsync([
            new TodoTask
            {
                Id = Guid.NewGuid(),
                ListId = listId,
                Title = "Keep",
                Description = "",
                Status = TaskStatus.Todo,
                Priority = Priority.Medium,
                SortOrder = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new TodoTask
            {
                Id = Guid.NewGuid(),
                ListId = missingListId,
                Title = "Skip",
                Description = "",
                Status = TaskStatus.Todo,
                Priority = Priority.Medium,
                SortOrder = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        ]);

        var result = await _importer.ImportAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result.TasksImported, Is.EqualTo(1));
            Assert.That(result.TasksSkipped, Is.EqualTo(1));
            Assert.That(_context.Tasks.IgnoreQueryFilters().Count(), Is.EqualTo(1));
        });
    }

    private async Task WriteListsAsync(List<TodoList> lists)
    {
        await File.WriteAllTextAsync(_listsFile, JsonSerializer.Serialize(lists));
    }

    private async Task WriteTasksAsync(List<TodoTask> tasks)
    {
        await File.WriteAllTextAsync(_tasksFile, JsonSerializer.Serialize(tasks));
    }
}
