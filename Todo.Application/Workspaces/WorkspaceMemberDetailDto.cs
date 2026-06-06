using Todo.Domain.Enums;

namespace Todo.Application.Workspaces;

public record WorkspaceMemberDetailDto(
    Guid UserId,
    string Name,
    string Email,
    WorkspaceRole Role,
    DateTime JoinedAt);
