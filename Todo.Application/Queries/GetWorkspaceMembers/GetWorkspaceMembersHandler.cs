using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Workspaces;

namespace Todo.Application.Queries.GetWorkspaceMembers;

public class GetWorkspaceMembersHandler
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly WorkspaceMembershipChecker _membership;

    public GetWorkspaceMembersHandler(
        IWorkspaceRepository workspaceRepository,
        WorkspaceMembershipChecker membership)
    {
        _workspaceRepository = workspaceRepository;
        _membership = membership;
    }

    public async Task<Result<List<WorkspaceMemberDetailDto>>> HandleAsync(
        GetWorkspaceMembersQuery query)
    {
        var memberResult = await _membership.RequireMemberAsync(query.WorkspaceId, query.UserId);
        if (!memberResult.IsSuccess)
            return Result<List<WorkspaceMemberDetailDto>>.Failure(memberResult.Error!);

        return await _workspaceRepository.GetMemberDetailsAsync(query.WorkspaceId);
    }
}
