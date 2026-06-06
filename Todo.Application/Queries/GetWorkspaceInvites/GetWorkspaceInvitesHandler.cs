using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Workspaces;

namespace Todo.Application.Queries.GetWorkspaceInvites;

public class GetWorkspaceInvitesHandler
{
    private readonly IWorkspaceInviteRepository _inviteRepository;
    private readonly WorkspaceMembershipChecker _membership;

    public GetWorkspaceInvitesHandler(
        IWorkspaceInviteRepository inviteRepository,
        WorkspaceMembershipChecker membership)
    {
        _inviteRepository = inviteRepository;
        _membership = membership;
    }

    public async Task<Result<List<WorkspaceInviteDto>>> HandleAsync(GetWorkspaceInvitesQuery query)
    {
        var ownerResult = await _membership.RequireOwnerAsync(query.WorkspaceId, query.UserId);
        if (!ownerResult.IsSuccess)
            return Result<List<WorkspaceInviteDto>>.Failure(ownerResult.Error!);

        var invitesResult = await _inviteRepository.GetPendingByWorkspaceIdAsync(query.WorkspaceId);
        if (!invitesResult.IsSuccess)
            return Result<List<WorkspaceInviteDto>>.Failure(invitesResult.Error!);

        var dtos = invitesResult.Value!
            .Select(invite => new WorkspaceInviteDto(
                invite.Id,
                invite.Email,
                invite.Role,
                invite.ExpiresAt))
            .ToList();

        return Result<List<WorkspaceInviteDto>>.Success(dtos);
    }
}
