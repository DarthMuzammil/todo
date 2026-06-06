using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Infrastructure.Repositories;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Tests.Repositories;

[TestFixture]
public class JsonTaskRepositoryTests
{
    private string _filePath = null!;
    private JsonTaskRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _filePath = Path.Combine(Path.GetTempPath(), $"tasks-test-{Guid.NewGuid()}.json");
        _repository = new JsonTaskRepository(_filePath);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Test]
    public async Task AddAsync_ThenGetById_ReturnsTask()
    {
        var task = new TodoTask
        {
            Id = Guid.NewGuid(),
            ListId = Guid.NewGuid(),
            Title = "Test task",
            Description = "Integration test",
            Status = TaskStatus.Todo,
            Priority = Priority.Medium,
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var addResult = await _repository.AddAsync(task);

        Assert.That(addResult.IsSuccess, Is.True);

        var getResult = await _repository.GetTaskByIdAsync(task.Id);

        Assert.That(getResult.IsSuccess, Is.True);
        Assert.That(getResult.Value!.Title, Is.EqualTo("Test task"));
        Assert.That(getResult.Value.ListId, Is.EqualTo(task.ListId));
    }

    [Test]
    public async Task GetById_WhenTaskDoesNotExist_ReturnsFailure()
    {
        var result = await _repository.GetTaskByIdAsync(Guid.NewGuid());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Task not found"));
    }

    [Test]
    public async Task RemoveAsync_SoftDeletesTask_GetByIdReturnsFailure()
    {
        var task = new TodoTask
        {
            Id = Guid.NewGuid(),
            ListId = Guid.NewGuid(),
            Title = "To delete",
            Description = "",
            Status = TaskStatus.Todo,
            Priority = Priority.Low,
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(task);

        var removeResult = await _repository.RemoveAsync(task.Id);

        Assert.That(removeResult.IsSuccess, Is.True);
        Assert.That(removeResult.Value!.IsDeleted, Is.True);

        var getResult = await _repository.GetTaskByIdAsync(task.Id);

        Assert.That(getResult.IsSuccess, Is.False);
        Assert.That(getResult.Error, Is.EqualTo("Task not found"));
    }
}
