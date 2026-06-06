using Todo.Domain.Enums;

namespace Todo.Application.Workspaces;

public record WorkspaceInviteResult(
    Guid Id,
    Guid WorkspaceId,
    string Email,
    WorkspaceRole Role,
    string Token,
    DateTime ExpiresAt);

public record WorkspaceMemberDto(
    Guid UserId,
    WorkspaceRole Role,
    DateTime JoinedAt);

public record WorkspaceInviteDto(
    Guid Id,
    string Email,
    WorkspaceRole Role,
    DateTime ExpiresAt);

public record WorkspaceSummaryDto(
    Guid Id,
    string Name,
    bool IsPersonal,
    WorkspaceRole CurrentUserRole);
