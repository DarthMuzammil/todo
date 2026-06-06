using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Enums;

namespace Todo.Application.Commands.ResendWorkspaceInvite;

public class ResendWorkspaceInviteHandler
{
    private const int InviteExpiryDays = 7;

    private readonly IWorkspaceInviteRepository _inviteRepository;
    private readonly IInviteTokenService _inviteTokenService;
    private readonly WorkspaceMembershipChecker _membership;
    private readonly IUnitOfWork _unitOfWork;

    public ResendWorkspaceInviteHandler(
        IWorkspaceInviteRepository inviteRepository,
        IInviteTokenService inviteTokenService,
        WorkspaceMembershipChecker membership,
        IUnitOfWork unitOfWork)
    {
        _inviteRepository = inviteRepository;
        _inviteTokenService = inviteTokenService;
        _membership = membership;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ResendWorkspaceInviteResult>> HandleAsync(
        ResendWorkspaceInviteCommand command)
    {
        var ownerResult = await _membership.RequireOwnerAsync(
            command.WorkspaceId,
            command.InviterUserId);

        if (!ownerResult.IsSuccess)
            return Result<ResendWorkspaceInviteResult>.Failure(ownerResult.Error!);

        var inviteResult = await _inviteRepository.GetByIdAsync(command.InviteId);
        if (!inviteResult.IsSuccess)
            return Result<ResendWorkspaceInviteResult>.Failure("Invite not found.");

        var invite = inviteResult.Value!;

        if (invite.WorkspaceId != command.WorkspaceId)
            return Result<ResendWorkspaceInviteResult>.Failure("Invite not found.");

        if (invite.Status != WorkspaceInviteStatus.Pending)
            return Result<ResendWorkspaceInviteResult>.Failure("Invite is no longer pending.");

        var plainToken = _inviteTokenService.GenerateToken();
        var now = DateTime.UtcNow;

        invite.TokenHash = _inviteTokenService.HashToken(plainToken);
        invite.ExpiresAt = now.AddDays(InviteExpiryDays);

        var updateResult = await _inviteRepository.UpdateAsync(invite);
        if (!updateResult.IsSuccess)
            return Result<ResendWorkspaceInviteResult>.Failure(updateResult.Error!);

        await _unitOfWork.CommitAsync();

        return Result<ResendWorkspaceInviteResult>.Success(
            new ResendWorkspaceInviteResult(
                invite.Id,
                invite.WorkspaceId,
                invite.Email,
                invite.Role,
                plainToken,
                invite.ExpiresAt));
    }
}
