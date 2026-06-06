using Todo.Application.Common;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Interfaces;

public interface IWorkspaceInviteRepository
{
    Task<Result<WorkspaceInvite>> GetByTokenHashAsync(string tokenHash);
    Task<Result<WorkspaceInvite>> GetByIdAsync(Guid inviteId);
    Task<Result<WorkspaceInvite>> GetPendingByWorkspaceAndEmailAsync(
        Guid workspaceId,
        string email);
    Task<Result<List<WorkspaceInvite>>> GetPendingByWorkspaceIdAsync(Guid workspaceId);
    Task<Result<WorkspaceInvite>> AddAsync(WorkspaceInvite invite);
    Task<Result<WorkspaceInvite>> UpdateAsync(WorkspaceInvite invite);
}
