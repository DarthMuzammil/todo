using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Infrastructure;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Repositories;
using Todo.Tests.Support;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Tests.Repositories;

[TestFixture]
public class EfTaskRepositoryTests
{
    private string _dbPath = null!;
    private TodoDbContext _context = null!;
    private EfTaskRepository _repository = null!;
    private UnitOfWork _unitOfWork = null!;

    [SetUp]
    public async Task SetUp()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"tasks-ef-{Guid.NewGuid():N}.db");

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        _context = new TodoDbContext(options);
        await _context.Database.MigrateAsync();

        _repository = new EfTaskRepository(_context);
        _unitOfWork = new UnitOfWork(_context);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    private Task<Guid> SeedUserAsync() => ApplicationUserTestFactory.SeedAsync(_context);

    private async Task<Guid> SeedListAsync(Guid ownerId)
    {
        var list = await WorkspaceTestFactory.SeedListAsync(_context, ownerId);
        return list.Id;
    }

    private static TodoTask CreateTask(Guid listId, string title = "Test task") =>
        new()
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            Title = title,
            Description = "Integration test",
            Status = TaskStatus.Todo,
            Priority = Priority.Medium,
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

    [Test]
    public async Task AddAsync_ThenGetById_ReturnsTask()
    {
        var ownerId = await SeedUserAsync();
        var listId = await SeedListAsync(ownerId);
        var task = CreateTask(listId);

        await _repository.AddAsync(task);
        await _unitOfWork.CommitAsync();

        var getResult = await _repository.GetTaskByIdAsync(task.Id);

        Assert.That(getResult.IsSuccess, Is.True);
        Assert.That(getResult.Value!.Title, Is.EqualTo("Test task"));
        Assert.That(getResult.Value.ListId, Is.EqualTo(listId));
    }

    [Test]
    public async Task GetTaskByIdAsync_WhenTaskDoesNotExist_ReturnsFailure()
    {
        var result = await _repository.GetTaskByIdAsync(Guid.NewGuid());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Task not found"));
    }

    [Test]
    public async Task GetByListIdAsync_ReturnsOnlyNonDeletedTasksForThatList()
    {
        var ownerId = await SeedUserAsync();
        var listA = await SeedListAsync(ownerId);
        var listB = await SeedListAsync(ownerId);

        var taskOnA = CreateTask(listA, "On A");
        var taskOnB = CreateTask(listB, "On B");
        var deletedOnA = CreateTask(listA, "Deleted on A");

        await _repository.AddAsync(taskOnA);
        await _repository.AddAsync(taskOnB);
        await _repository.AddAsync(deletedOnA);
        await _unitOfWork.CommitAsync();

        await _repository.RemoveAsync(deletedOnA.Id);
        await _unitOfWork.CommitAsync();

        var result = await _repository.GetByListIdAsync(listA);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(1));
        Assert.That(result.Value![0].Title, Is.EqualTo("On A"));
    }

    [Test]
    public async Task UpdateAsync_ChangesTitle_GetByIdReturnsUpdated()
    {
        var ownerId = await SeedUserAsync();
        var listId = await SeedListAsync(ownerId);
        var task = CreateTask(listId, "Original");

        await _repository.AddAsync(task);
        await _unitOfWork.CommitAsync();

        task.Title = "Updated";
        task.UpdatedAt = DateTime.UtcNow.AddMinutes(1);

        var updateResult = await _repository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();

        var getResult = await _repository.GetTaskByIdAsync(task.Id);

        Assert.That(updateResult.IsSuccess, Is.True);
        Assert.That(getResult.IsSuccess, Is.True);
        Assert.That(getResult.Value!.Title, Is.EqualTo("Updated"));
    }

    [Test]
    public async Task UpdateAsync_WhenTaskDoesNotExist_ReturnsFailure()
    {
        var task = CreateTask(Guid.NewGuid(), "Missing");

        var result = await _repository.UpdateAsync(task);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Task not found"));
    }

    [Test]
    public async Task RemoveAsync_SoftDeletesTask_GetByIdReturnsFailure()
    {
        var ownerId = await SeedUserAsync();
        var listId = await SeedListAsync(ownerId);
        var task = CreateTask(listId, "To delete");

        await _repository.AddAsync(task);
        await _unitOfWork.CommitAsync();

        var removeResult = await _repository.RemoveAsync(task.Id);
        await _unitOfWork.CommitAsync();

        var getResult = await _repository.GetTaskByIdAsync(task.Id);

        Assert.That(removeResult.IsSuccess, Is.True);
        Assert.That(removeResult.Value!.IsDeleted, Is.True);
        Assert.That(removeResult.Value.DeletedAt, Is.Not.Null);
        Assert.That(getResult.IsSuccess, Is.False);
        Assert.That(getResult.Error, Is.EqualTo("Task not found"));
    }

    [Test]
    public async Task RemoveAsync_WhenTaskDoesNotExist_ReturnsFailure()
    {
        var result = await _repository.RemoveAsync(Guid.NewGuid());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Task not found"));
    }
}
