using Microsoft.EntityFrameworkCore;
using Todo.Application.Activity;
using Todo.Application.Commands.CreateTask;
using Todo.Domain.Enums;
using Todo.Infrastructure;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Repositories;
using Todo.Tests.Support;

namespace Todo.Tests.Activity;

[TestFixture]
public class ActivityRecorderTests
{
    private string _dbPath = null!;
    private TodoDbContext _context = null!;
    private ActivityRecorder _recorder = null!;
    private UnitOfWork _unitOfWork = null!;

    [SetUp]
    public async Task SetUp()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"activity-{Guid.NewGuid():N}.db");

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        _context = new TodoDbContext(options);
        await _context.Database.MigrateAsync();

        var workspaceRepository = new EfWorkspaceRepository(_context);
        _unitOfWork = new UnitOfWork(_context);
        _recorder = new ActivityRecorder(
            new EfActivityRepository(_context),
            new EfNotificationRepository(_context),
            workspaceRepository,
            new TestUserDirectory());
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Test]
    public async Task RecordTaskCreated_WritesActivityAndNotificationForOtherMember()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context, name: "Owner");
        var editorId = await ApplicationUserTestFactory.SeedAsync(_context, name: "Editor");
        var (workspaceId, _) = await WorkspaceTestFactory.SeedSharedWorkspaceAsync(_context, ownerId);
        await WorkspaceTestFactory.AddMemberAsync(_context, workspaceId, editorId, WorkspaceRole.Editor);
        var list = await WorkspaceTestFactory.SeedListInWorkspaceAsync(_context, ownerId, workspaceId);

        var task = new Todo.Domain.Entities.TodoTask
        {
            Id = Guid.NewGuid(),
            ListId = list.Id,
            Title = "Buy milk",
            Description = string.Empty,
            Status = Todo.Domain.Enums.TaskStatus.Todo,
            Priority = Priority.Medium,
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false,
            Version = 1,
        };

        await _recorder.RecordTaskCreatedAsync(task, list, ownerId);
        await _unitOfWork.CommitAsync();

        var activityCount = await _context.ActivityEntries.CountAsync();
        var notification = await _context.UserNotifications
            .SingleAsync(item => item.UserId == editorId);

        Assert.That(activityCount, Is.EqualTo(1));
        Assert.That(notification.Message, Does.Contain("Buy milk"));
        Assert.That(notification.ListId, Is.EqualTo(list.Id));
    }
}
