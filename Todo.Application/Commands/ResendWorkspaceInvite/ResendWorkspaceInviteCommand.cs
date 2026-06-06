using Todo.Domain.Enums;

namespace Todo.Application.Commands.ResendWorkspaceInvite;

public record ResendWorkspaceInviteCommand(
    Guid WorkspaceId,
    Guid InviterUserId,
    Guid InviteId);

public record ResendWorkspaceInviteResult(
    Guid Id,
    Guid WorkspaceId,
    string Email,
    WorkspaceRole Role,
    string Token,
    DateTime ExpiresAt);
