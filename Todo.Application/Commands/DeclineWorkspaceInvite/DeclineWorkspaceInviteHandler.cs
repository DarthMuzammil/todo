using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Commands.DeclineWorkspaceInvite;

public class DeclineWorkspaceInviteHandler
{
    private readonly IWorkspaceInviteRepository _inviteRepository;
    private readonly IInviteTokenService _inviteTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public DeclineWorkspaceInviteHandler(
        IWorkspaceInviteRepository inviteRepository,
        IInviteTokenService inviteTokenService,
        IUnitOfWork unitOfWork)
    {
        _inviteRepository = inviteRepository;
        _inviteTokenService = inviteTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkspaceInvite>> HandleAsync(DeclineWorkspaceInviteCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Token))
            return Result<WorkspaceInvite>.Failure("Invite token is required.");

        var inviteResult = await _inviteRepository.GetByTokenHashAsync(
            _inviteTokenService.HashToken(command.Token));

        if (!inviteResult.IsSuccess)
            return Result<WorkspaceInvite>.Failure("Invite not found.");

        var invite = inviteResult.Value!;

        if (invite.Status != WorkspaceInviteStatus.Pending)
            return Result<WorkspaceInvite>.Failure("Invite is no longer pending.");

        if (invite.ExpiresAt <= DateTime.UtcNow)
            return Result<WorkspaceInvite>.Failure("Invite has expired.");

        var normalizedEmail = command.UserEmail.Trim().ToLowerInvariant();
        if (!string.Equals(invite.Email, normalizedEmail, StringComparison.Ordinal))
            return Result<WorkspaceInvite>.Failure("This invite was sent to a different email address.");

        invite.Status = WorkspaceInviteStatus.Declined;
        invite.RespondedAt = DateTime.UtcNow;

        var updateResult = await _inviteRepository.UpdateAsync(invite);
        if (!updateResult.IsSuccess)
            return updateResult;

        await _unitOfWork.CommitAsync();
        return updateResult;
    }
}
