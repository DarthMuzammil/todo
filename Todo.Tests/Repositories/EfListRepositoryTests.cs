using Microsoft.EntityFrameworkCore;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Infrastructure;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Repositories;
using Todo.Tests.Support;

namespace Todo.Tests.Repositories;

[TestFixture]
public class EfListRepositoryTests
{
    private string _dbPath = null!;
    private TodoDbContext _context = null!;
    private IListRepository _repository = null!;
    private UnitOfWork _unitOfWork = null!;

    [SetUp]
    public async Task SetUp()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"lists-ef-{Guid.NewGuid():N}.db");

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        _context = new TodoDbContext(options);
        await _context.Database.MigrateAsync();

        _repository = new EfListRepository(_context);
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

    [Test]
    public async Task AddAsync_ThenGetById_ReturnsList()
    {
        var ownerId = await SeedUserAsync();
        var workspaceId = await WorkspaceTestFactory.SeedPersonalWorkspaceAsync(_context, ownerId);

        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            WorkspaceId = workspaceId,
            Title = "Test list",
            Color = "#ffffff",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(list);
        await _unitOfWork.CommitAsync();

        var getResult = await _repository.GetByIdAsync(list.Id);

        Assert.That(getResult.IsSuccess, Is.True);
        Assert.That(getResult.Value!.Title, Is.EqualTo("Test list"));
    }

    [Test]
    public async Task GetByOwnerIdAsync_ReturnsNonDeletedListsSortedByUpdatedAtDesc()
    {
        var ownerId = await SeedUserAsync();
        var otherOwnerId = await SeedUserAsync();
        var ownerWorkspaceId = await WorkspaceTestFactory.SeedPersonalWorkspaceAsync(_context, ownerId);
        var otherWorkspaceId = await WorkspaceTestFactory.SeedPersonalWorkspaceAsync(_context, otherOwnerId);

        var older = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            WorkspaceId = ownerWorkspaceId,
            Title = "Older",
            Color = "#111111",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
            IsDeleted = false
        };

        var newer = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            WorkspaceId = ownerWorkspaceId,
            Title = "Newer",
            Color = "#222222",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var deleted = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            WorkspaceId = ownerWorkspaceId,
            Title = "Deleted",
            Color = "#333333",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var otherOwnerList = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = otherOwnerId,
            WorkspaceId = otherWorkspaceId,
            Title = "Other owner",
            Color = "#444444",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(older);
        await _repository.AddAsync(newer);
        await _repository.AddAsync(deleted);
        await _repository.AddAsync(otherOwnerList);
        await _unitOfWork.CommitAsync();

        await _repository.RemoveAsync(deleted.Id);
        await _unitOfWork.CommitAsync();

        var result = await _repository.GetByOwnerIdAsync(ownerId);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(2));
        Assert.That(result.Value![0].Title, Is.EqualTo("Newer"));
        Assert.That(result.Value[1].Title, Is.EqualTo("Older"));
    }

    [Test]
    public async Task RemoveAsync_SoftDeletesList_GetByIdReturnsFailure()
    {
        var ownerId = await SeedUserAsync();
        var workspaceId = await WorkspaceTestFactory.SeedPersonalWorkspaceAsync(_context, ownerId);

        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            WorkspaceId = workspaceId,
            Title = "To delete",
            Color = "#000000",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(list);
        await _unitOfWork.CommitAsync();

        await _repository.RemoveAsync(list.Id);
        await _unitOfWork.CommitAsync();

        var getResult = await _repository.GetByIdAsync(list.Id);

        Assert.That(getResult.IsSuccess, Is.False);
        Assert.That(getResult.Error, Is.EqualTo("List not found"));
    }
}