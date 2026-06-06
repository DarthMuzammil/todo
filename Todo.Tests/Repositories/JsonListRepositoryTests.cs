using Todo.Domain.Entities;
using Todo.Infrastructure.Repositories;

namespace Todo.Tests.Repositories;

[TestFixture]
public class JsonListRepositoryTests
{
    private string _filePath = null!;
    private JsonListRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _filePath = Path.Combine(Path.GetTempPath(), $"lists-test-{Guid.NewGuid()}.json");
        _repository = new JsonListRepository(_filePath);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Test]
    public async Task AddAsync_ThenGetById_ReturnsList()
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
            Title = "Test list",
            Color = "#ffffff",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var addResult = await _repository.AddAsync(list);

        Assert.That(addResult.IsSuccess, Is.True);

        var getResult = await _repository.GetByIdAsync(list.Id);

        Assert.That(getResult.IsSuccess, Is.True);
        Assert.That(getResult.Value!.Title, Is.EqualTo("Test list"));
        Assert.That(getResult.Value.OwnerId, Is.EqualTo(list.OwnerId));
    }

    [Test]
    public async Task GetById_WhenListDoesNotExist_ReturnsFailure()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("List not found"));
    }

    [Test]
    public async Task GetByOwnerId_ReturnsOnlyThatOwnersLists()
    {
        var ownerA = Guid.NewGuid();
        var ownerB = Guid.NewGuid();

        var list1 = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerA,
            Title = "Test list",
            Color = "#ffffff",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        var list2 = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerA,
            Title = "Test list",
            Color = "#ffffff",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        var list3 = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerB,
            Title = "Test list",
            Color = "#ffffff",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _repository.AddAsync(list1);
        await _repository.AddAsync(list2);
        await _repository.AddAsync(list3);

        var result = await _repository.GetByOwnerIdAsync(ownerA);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(2));
        Assert.That(result.Value!.Select(l => l.Id), Is.EquivalentTo(new[] { list1.Id, list2.Id }));
    }

    [Test]
    public async Task RemoveAsync_SoftDeletesList_GetByIdReturnsFailure()
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
            Title = "To delete",
            Color = "#000000",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(list);

        var removeResult = await _repository.RemoveAsync(list.Id);

        Assert.That(removeResult.IsSuccess, Is.True);
        Assert.That(removeResult.Value!.IsDeleted, Is.True);

        var getResult = await _repository.GetByIdAsync(list.Id);

        Assert.That(getResult.IsSuccess, Is.False);
        Assert.That(getResult.Error, Is.EqualTo("List not found"));
    }

    [Test]
    public async Task UpdateAsync_ChangesTitle_GetByIdReturnsUpdated()
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
            Title = "Original",
            Color = "#000000",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _repository.AddAsync(list);
        list.Title = "Updated";
        var updatedResult = await _repository.UpdateAsync(list);
        var getResult = await _repository.GetByIdAsync(list.Id);
        Assert.That(updatedResult.IsSuccess, Is.True);
        Assert.That(getResult.Value!.Title, Is.EqualTo("Updated"));
    }
}
