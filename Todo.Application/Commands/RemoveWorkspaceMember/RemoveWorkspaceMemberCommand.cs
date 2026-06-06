namespace Todo.Application.Commands.RemoveWorkspaceMember;

public record RemoveWorkspaceMemberCommand(
    Guid WorkspaceId,
    Guid RequesterUserId,
    Guid MemberUserId);
