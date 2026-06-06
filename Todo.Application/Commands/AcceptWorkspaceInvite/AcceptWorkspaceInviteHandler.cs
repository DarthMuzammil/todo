using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Commands.AcceptWorkspaceInvite;

public class AcceptWorkspaceInviteHandler
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IWorkspaceInviteRepository _inviteRepository;
    private readonly IInviteTokenService _inviteTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptWorkspaceInviteHandler(
        IWorkspaceRepository workspaceRepository,
        IWorkspaceInviteRepository inviteRepository,
        IInviteTokenService inviteTokenService,
        IUnitOfWork unitOfWork)
    {
        _workspaceRepository = workspaceRepository;
        _inviteRepository = inviteRepository;
        _inviteTokenService = inviteTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkspaceMember>> HandleAsync(AcceptWorkspaceInviteCommand command)
    {
        var inviteResult = await GetValidPendingInviteAsync(command.Token);
        if (!inviteResult.IsSuccess)
            return Result<WorkspaceMember>.Failure(inviteResult.Error!);

        var invite = inviteResult.Value!;
        var normalizedEmail = command.UserEmail.Trim().ToLowerInvariant();

        if (!string.Equals(invite.Email, normalizedEmail, StringComparison.Ordinal))
            return Result<WorkspaceMember>.Failure("This invite was sent to a different email address.");

        var existingMember = await _workspaceRepository.GetMemberAsync(invite.WorkspaceId, command.UserId);
        if (existingMember.IsSuccess)
            return Result<WorkspaceMember>.Failure("You are already a member of this workspace.");

        var addResult = await _workspaceRepository.AddMemberAsync(
            invite.WorkspaceId,
            command.UserId,
            invite.Role);

        if (!addResult.IsSuccess)
            return addResult;

        invite.Status = WorkspaceInviteStatus.Accepted;
        invite.RespondedAt = DateTime.UtcNow;

        var updateResult = await _inviteRepository.UpdateAsync(invite);
        if (!updateResult.IsSuccess)
            return Result<WorkspaceMember>.Failure(updateResult.Error!);

        await _unitOfWork.CommitAsync();
        return addResult;
    }

    private async Task<Result<WorkspaceInvite>> GetValidPendingInviteAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Result<WorkspaceInvite>.Failure("Invite token is required.");

        var inviteResult = await _inviteRepository.GetByTokenHashAsync(
            _inviteTokenService.HashToken(token));

        if (!inviteResult.IsSuccess)
            return Result<WorkspaceInvite>.Failure("Invite not found.");

        var invite = inviteResult.Value!;

        if (invite.Status != WorkspaceInviteStatus.Pending)
            return Result<WorkspaceInvite>.Failure("Invite is no longer pending.");

        if (invite.ExpiresAt <= DateTime.UtcNow)
            return Result<WorkspaceInvite>.Failure("Invite has expired.");

        return inviteResult;
    }
}
