using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Enums;

namespace Todo.Application.Commands.RemoveWorkspaceMember;

public class RemoveWorkspaceMemberHandler
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly WorkspaceMembershipChecker _membership;

    public RemoveWorkspaceMemberHandler(
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork,
        WorkspaceMembershipChecker membership)
    {
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
        _membership = membership;
    }

    public async Task<Result> HandleAsync(RemoveWorkspaceMemberCommand command)
    {
        var ownerResult = await _membership.RequireOwnerAsync(
            command.WorkspaceId,
            command.RequesterUserId);

        if (!ownerResult.IsSuccess)
            return Result.Failure(ownerResult.Error!);

        if (command.MemberUserId == command.RequesterUserId)
            return Result.Failure("You cannot remove yourself from the workspace.");

        var memberResult = await _workspaceRepository.GetMemberAsync(
            command.WorkspaceId,
            command.MemberUserId);

        if (!memberResult.IsSuccess)
            return Result.Failure("Member not found.");

        if (memberResult.Value!.Role == WorkspaceRole.Owner)
            return Result.Failure("Cannot remove the workspace owner.");

        var removeResult = await _workspaceRepository.RemoveMemberAsync(
            command.WorkspaceId,
            command.MemberUserId);

        if (!removeResult.IsSuccess)
            return removeResult;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
