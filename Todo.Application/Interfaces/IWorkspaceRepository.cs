using Todo.Application.Common;
using Todo.Application.Workspaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Interfaces;

public interface IWorkspaceRepository
{
    Task<Result<Workspace>> GetByIdAsync(Guid workspaceId);
    Task<Result<Workspace>> GetPersonalWorkspaceByUserIdAsync(Guid userId);
    Task<Result<List<Workspace>>> GetWorkspacesForUserAsync(Guid userId);
    Task<Result<WorkspaceMember>> GetMemberAsync(Guid workspaceId, Guid userId);
    Task<Result<Workspace>> CreatePersonalWorkspaceAsync(Guid userId);
    Task<Result<Workspace>> CreateSharedWorkspaceAsync(Guid userId, string name);
    Task<Result<WorkspaceMember>> AddMemberAsync(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole role);
    Task<Result<List<WorkspaceMemberDetailDto>>> GetMemberDetailsAsync(Guid workspaceId);
    Task<Result<List<Guid>>> GetMemberUserIdsAsync(Guid workspaceId);
    Task<Result> RemoveMemberAsync(Guid workspaceId, Guid userId);
    Task<bool> IsMemberByEmailAsync(Guid workspaceId, string email);
}
