using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Common;

public class WorkspaceMembershipChecker
{
    public const string WorkspaceNotFoundMessage = "Workspace not found";
    public const string NotMemberMessage = "Workspace not found";

    private readonly IWorkspaceRepository _workspaceRepository;

    public WorkspaceMembershipChecker(IWorkspaceRepository workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Result<Workspace>> RequireExistingWorkspaceAsync(Guid workspaceId)
    {
        var result = await _workspaceRepository.GetByIdAsync(workspaceId);
        if (!result.IsSuccess)
            return Result<Workspace>.Failure(WorkspaceNotFoundMessage);

        return result;
    }

    public async Task<Result<WorkspaceMember>> RequireMemberAsync(Guid workspaceId, Guid userId)
    {
        var workspaceResult = await RequireExistingWorkspaceAsync(workspaceId);
        if (!workspaceResult.IsSuccess)
            return Result<WorkspaceMember>.Failure(workspaceResult.Error!);

        var memberResult = await _workspaceRepository.GetMemberAsync(workspaceId, userId);
        if (!memberResult.IsSuccess)
            return Result<WorkspaceMember>.Failure(NotMemberMessage);

        return memberResult;
    }

    public async Task<Result<WorkspaceMember>> RequireOwnerAsync(Guid workspaceId, Guid userId)
    {
        var memberResult = await RequireMemberAsync(workspaceId, userId);
        if (!memberResult.IsSuccess)
            return memberResult;

        if (memberResult.Value!.Role != WorkspaceRole.Owner)
            return Result<WorkspaceMember>.Failure(NotMemberMessage);

        return memberResult;
    }
}
