using Microsoft.EntityFrameworkCore;
using Todo.Application.Commands.AcceptWorkspaceInvite;
using Todo.Application.Commands.CreateSharedWorkspace;
using Todo.Application.Commands.DeclineWorkspaceInvite;
using Todo.Application.Commands.ResendWorkspaceInvite;
using Todo.Application.Commands.SendWorkspaceInvite;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Enums;
using Todo.Infrastructure;
using Todo.Infrastructure.Identity;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Repositories;
using Todo.Tests.Support;

namespace Todo.Tests.Workspaces;

[TestFixture]
public class WorkspaceInviteHandlerTests
{
    private string _dbPath = null!;
    private TodoDbContext _context = null!;
    private UnitOfWork _unitOfWork = null!;
    private IWorkspaceRepository _workspaceRepository = null!;
    private IWorkspaceInviteRepository _inviteRepository = null!;
    private IInviteTokenService _inviteTokenService = null!;
    private WorkspaceMembershipChecker _membership = null!;
    private CreateSharedWorkspaceHandler _createSharedWorkspaceHandler = null!;
    private SendWorkspaceInviteHandler _sendInviteHandler = null!;
    private AcceptWorkspaceInviteHandler _acceptInviteHandler = null!;
    private DeclineWorkspaceInviteHandler _declineInviteHandler = null!;

    [SetUp]
    public async Task SetUp()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"workspace-invite-{Guid.NewGuid():N}.db");

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        _context = new TodoDbContext(options);
        await _context.Database.MigrateAsync();

        _unitOfWork = new UnitOfWork(_context);
        _workspaceRepository = new EfWorkspaceRepository(_context);
        _inviteRepository = new EfWorkspaceInviteRepository(_context);
        _inviteTokenService = new InviteTokenService();
        _membership = new WorkspaceMembershipChecker(_workspaceRepository);

        _createSharedWorkspaceHandler = new CreateSharedWorkspaceHandler(
            _workspaceRepository,
            _unitOfWork);

        _sendInviteHandler = new SendWorkspaceInviteHandler(
            _workspaceRepository,
            _inviteRepository,
            _inviteTokenService,
            _membership,
            _unitOfWork);

        _acceptInviteHandler = new AcceptWorkspaceInviteHandler(
            _workspaceRepository,
            _inviteRepository,
            _inviteTokenService,
            _unitOfWork);

        _declineInviteHandler = new DeclineWorkspaceInviteHandler(
            _inviteRepository,
            _inviteTokenService,
            _unitOfWork);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Test]
    public async Task SendInvite_ThenAccept_AddsWorkspaceMember()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context, name: "Owner");
        var inviteeId = await ApplicationUserTestFactory.SeedAsync(_context, name: "Invitee");
        var invitee = await _context.Users.FindAsync(inviteeId);

        var createResult = await _createSharedWorkspaceHandler.HandleAsync(
            new CreateSharedWorkspaceCommand(ownerId, "Family"));

        Assert.That(createResult.IsSuccess, Is.True);

        var sendResult = await _sendInviteHandler.HandleAsync(
            new SendWorkspaceInviteCommand(
                createResult.Value!.Id,
                ownerId,
                invitee!.Email!,
                WorkspaceRole.Editor));

        Assert.That(sendResult.IsSuccess, Is.True);

        var acceptResult = await _acceptInviteHandler.HandleAsync(
            new AcceptWorkspaceInviteCommand(
                inviteeId,
                invitee.Email!,
                sendResult.Value!.Token));

        Assert.That(acceptResult.IsSuccess, Is.True);
        Assert.That(acceptResult.Value!.Role, Is.EqualTo(WorkspaceRole.Editor));

        var memberResult = await _workspaceRepository.GetMemberAsync(
            createResult.Value.Id,
            inviteeId);

        Assert.That(memberResult.IsSuccess, Is.True);
    }

    [Test]
    public async Task SendInvite_ToPersonalWorkspace_ReturnsFailure()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var workspaceId = await WorkspaceTestFactory.SeedPersonalWorkspaceAsync(_context, ownerId);

        var sendResult = await _sendInviteHandler.HandleAsync(
            new SendWorkspaceInviteCommand(
                workspaceId,
                ownerId,
                "partner@example.com",
                WorkspaceRole.Viewer));

        Assert.That(sendResult.IsSuccess, Is.False);
        Assert.That(sendResult.Error, Does.Contain("personal workspace"));
    }

    [Test]
    public async Task DeclineInvite_MarksInviteDeclined()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var inviteeId = await ApplicationUserTestFactory.SeedAsync(_context);
        var invitee = await _context.Users.FindAsync(inviteeId);

        var createResult = await _createSharedWorkspaceHandler.HandleAsync(
            new CreateSharedWorkspaceCommand(ownerId, "Team"));

        var sendResult = await _sendInviteHandler.HandleAsync(
            new SendWorkspaceInviteCommand(
                createResult.Value!.Id,
                ownerId,
                invitee!.Email!,
                WorkspaceRole.Viewer));

        var declineResult = await _declineInviteHandler.HandleAsync(
            new DeclineWorkspaceInviteCommand(invitee.Email!, sendResult.Value!.Token));

        Assert.That(declineResult.IsSuccess, Is.True);
        Assert.That(declineResult.Value!.Status, Is.EqualTo(WorkspaceInviteStatus.Declined));

        var memberResult = await _workspaceRepository.GetMemberAsync(
            createResult.Value.Id,
            inviteeId);

        Assert.That(memberResult.IsSuccess, Is.False);
    }

    [Test]
    public async Task ResendInvite_ReturnsNewToken()
    {
        var ownerId = await ApplicationUserTestFactory.SeedAsync(_context);
        var inviteeId = await ApplicationUserTestFactory.SeedAsync(_context);
        var invitee = await _context.Users.FindAsync(inviteeId);

        var createResult = await _createSharedWorkspaceHandler.HandleAsync(
            new CreateSharedWorkspaceCommand(ownerId, "Team"));

        var sendResult = await _sendInviteHandler.HandleAsync(
            new SendWorkspaceInviteCommand(
                createResult.Value!.Id,
                ownerId,
                invitee!.Email!,
                WorkspaceRole.Viewer));

        var resendHandler = new ResendWorkspaceInviteHandler(
            _inviteRepository,
            _inviteTokenService,
            _membership,
            _unitOfWork);

        var resendResult = await resendHandler.HandleAsync(
            new ResendWorkspaceInviteCommand(
                createResult.Value.Id,
                ownerId,
                sendResult.Value!.Id));

        Assert.That(resendResult.IsSuccess, Is.True);
        Assert.That(resendResult.Value!.Token, Is.Not.EqualTo(sendResult.Value.Token));
    }
}
