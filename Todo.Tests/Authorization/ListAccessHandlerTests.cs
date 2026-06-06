using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.Activity;
using Todo.Application.Commands.CreateTask;
using Todo.Application.Realtime;
using Todo.Application.Queries.GetListById;
using Todo.Application.Queries.GetListsByOwnerId;
using Todo.Application.Queries.GetTasksByListId;
using Todo.Domain.Enums;
using Todo.Infrastructure;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Repositories;
using Todo.Tests.Support;

namespace Todo.Tests.Authorization;

[TestFixture]
public class ListAccessHandlerTests
{
    private string _dbPath = null!;
    private TodoDbContext _context = null!;
    private ListAccessChecker _access = null!;
    private GetListByIdHandler _getListByIdHandler = null!;
    private GetTasksByListIdHandler _getTasksHandler = null!;
    private GetListsByOwnerIdHandler _getListsHandler = null!;
    private CreateTaskHandler _createTaskHandler = null!;
    private UnitOfWork _unitOfWork = null!;

    [SetUp]
    public async Task SetUp()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"list-access-{Guid.NewGuid():N}.db");

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        _context = new TodoDbContext(options);
        await _context.Database.MigrateAsync();

        var listRepository = new EfListRepository(_context);
        var taskRepository = new EfTaskRepository(_context);
        var workspaceRepository = new EfWorkspaceRepository(_context);
        _unitOfWork = new UnitOfWork(_context);
        _access = new ListAccessChecker(listRepository, taskRepository, workspaceRepository);
        _getListByIdHandler = new GetListByIdHandler(_access);
        _getTasksHandler = new GetTasksByListIdHandler(taskRepository, _access);
        _getListsHandler = new GetListsByOwnerIdHandler(listRepository, workspaceRepository);
        _createTaskHandler = new CreateTaskHandler(
            taskRepository,
            _unitOfWork,
            _access,
            new NullListSyncNotifier(),
            new ActivityRecorder(
                new EfActivityRepository(_context),
                new EfNotificationRepository(_context),
                workspaceRepository,
                new TestUserDirectory()));
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Test]
    public async Task GetListById_WhenUserIsNotWorkspaceMember_ReturnsNotFound()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var otherUserId = await ApplicationUserTestFactory.SeedAsync(_context);
        var list = await WorkspaceTestFactory.SeedListAsync(_context, ownerId, "Private list");

        var result = await _getListByIdHandler.HandleAsync(new GetListByIdQuery(list.Id, otherUserId));

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo(ListAccessChecker.NotFoundMessage));
    }

    [Test]
    public async Task GetListById_WhenUserIsViewer_ReturnsSuccess()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var viewerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var (workspaceId, _) = await WorkspaceTestFactory.SeedSharedWorkspaceAsync(_context, ownerId);
        await WorkspaceTestFactory.AddMemberAsync(_context, workspaceId, viewerId, WorkspaceRole.Viewer);
        var list = await WorkspaceTestFactory.SeedListInWorkspaceAsync(_context, ownerId, workspaceId);

        var result = await _getListByIdHandler.HandleAsync(new GetListByIdQuery(list.Id, viewerId));

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task CreateTask_WhenUserIsViewer_ReturnsForbidden()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var viewerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var (workspaceId, _) = await WorkspaceTestFactory.SeedSharedWorkspaceAsync(_context, ownerId);
        await WorkspaceTestFactory.AddMemberAsync(_context, workspaceId, viewerId, WorkspaceRole.Viewer);
        var list = await WorkspaceTestFactory.SeedListInWorkspaceAsync(_context, ownerId, workspaceId);

        var result = await _createTaskHandler.HandleAsync(new CreateTaskCommand(
            list.Id,
            "Blocked task",
            null,
            Priority.Medium,
            null,
            viewerId));

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo(ListAccessChecker.ForbiddenMessage));
    }

    [Test]
    public async Task CreateTask_WhenUserIsEditor_ReturnsSuccess()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var editorId = await ApplicationUserTestFactory.SeedAsync(_context);
        var (workspaceId, _) = await WorkspaceTestFactory.SeedSharedWorkspaceAsync(_context, ownerId);
        await WorkspaceTestFactory.AddMemberAsync(_context, workspaceId, editorId, WorkspaceRole.Editor);
        var list = await WorkspaceTestFactory.SeedListInWorkspaceAsync(_context, ownerId, workspaceId);

        var result = await _createTaskHandler.HandleAsync(new CreateTaskCommand(
            list.Id,
            "Allowed task",
            null,
            Priority.Medium,
            null,
            editorId));

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GetLists_ReturnsListsFromAllMemberWorkspaces()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var memberId = await ApplicationUserTestFactory.SeedAsync(_context);
        var personalList = await WorkspaceTestFactory.SeedListAsync(_context, ownerId, "Personal list");
        var (workspaceId, _) = await WorkspaceTestFactory.SeedSharedWorkspaceAsync(_context, ownerId, "Team");
        var sharedList = await WorkspaceTestFactory.SeedListInWorkspaceAsync(
            _context,
            ownerId,
            workspaceId,
            "Team list");
        await WorkspaceTestFactory.AddMemberAsync(_context, workspaceId, memberId, WorkspaceRole.Viewer);

        var result = await _getListsHandler.HandleAsync(new GetListsByOwnerIdQuery(memberId));

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Select(list => list.Id), Does.Contain(sharedList.Id));
        Assert.That(result.Value.Select(list => list.Id), Does.Not.Contain(personalList.Id));
    }
}
