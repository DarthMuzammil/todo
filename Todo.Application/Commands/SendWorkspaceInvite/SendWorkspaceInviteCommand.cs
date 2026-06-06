using Todo.Domain.Enums;

namespace Todo.Application.Commands.SendWorkspaceInvite;

public record SendWorkspaceInviteCommand(
    Guid WorkspaceId,
    Guid InviterUserId,
    string Email,
    WorkspaceRole Role);
