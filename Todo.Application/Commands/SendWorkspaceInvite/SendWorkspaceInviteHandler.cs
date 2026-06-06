using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Workspaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Commands.SendWorkspaceInvite;

public class SendWorkspaceInviteHandler
{
    private const int InviteExpiryDays = 7;

    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IWorkspaceInviteRepository _inviteRepository;
    private readonly IInviteTokenService _inviteTokenService;
    private readonly WorkspaceMembershipChecker _membership;
    private readonly IUnitOfWork _unitOfWork;

    public SendWorkspaceInviteHandler(
        IWorkspaceRepository workspaceRepository,
        IWorkspaceInviteRepository inviteRepository,
        IInviteTokenService inviteTokenService,
        WorkspaceMembershipChecker membership,
        IUnitOfWork unitOfWork)
    {
        _workspaceRepository = workspaceRepository;
        _inviteRepository = inviteRepository;
        _inviteTokenService = inviteTokenService;
        _membership = membership;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkspaceInviteResult>> HandleAsync(SendWorkspaceInviteCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            return Result<WorkspaceInviteResult>.Failure("Email is required.");

        if (command.Role is WorkspaceRole.Owner)
            return Result<WorkspaceInviteResult>.Failure("Cannot invite a user as Owner.");

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var ownerResult = await _membership.RequireOwnerAsync(
            command.WorkspaceId,
            command.InviterUserId);

        if (!ownerResult.IsSuccess)
            return Result<WorkspaceInviteResult>.Failure(ownerResult.Error!);

        var workspaceResult = await _workspaceRepository.GetByIdAsync(command.WorkspaceId);
        if (!workspaceResult.IsSuccess)
            return Result<WorkspaceInviteResult>.Failure(workspaceResult.Error!);

        if (workspaceResult.Value!.IsPersonal)
            return Result<WorkspaceInviteResult>.Failure("Cannot invite users to a personal workspace.");

        if (await _workspaceRepository.IsMemberByEmailAsync(command.WorkspaceId, normalizedEmail))
            return Result<WorkspaceInviteResult>.Failure("User is already a member of this workspace.");

        var pendingResult = await _inviteRepository.GetPendingByWorkspaceAndEmailAsync(
            command.WorkspaceId,
            normalizedEmail);

        if (pendingResult.IsSuccess)
            return Result<WorkspaceInviteResult>.Failure("A pending invite already exists for this email.");

        var plainToken = _inviteTokenService.GenerateToken();
        var now = DateTime.UtcNow;

        var invite = new WorkspaceInvite
        {
            Id = Guid.NewGuid(),
            WorkspaceId = command.WorkspaceId,
            Email = normalizedEmail,
            Role = command.Role,
            TokenHash = _inviteTokenService.HashToken(plainToken),
            InvitedByUserId = command.InviterUserId,
            Status = WorkspaceInviteStatus.Pending,
            CreatedAt = now,
            ExpiresAt = now.AddDays(InviteExpiryDays)
        };

        var addResult = await _inviteRepository.AddAsync(invite);
        if (!addResult.IsSuccess)
            return Result<WorkspaceInviteResult>.Failure(addResult.Error!);

        await _unitOfWork.CommitAsync();

        return Result<WorkspaceInviteResult>.Success(
            new WorkspaceInviteResult(
                invite.Id,
                invite.WorkspaceId,
                invite.Email,
                invite.Role,
                plainToken,
                invite.ExpiresAt));
    }
}
