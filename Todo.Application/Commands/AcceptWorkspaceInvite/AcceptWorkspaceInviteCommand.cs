namespace Todo.Application.Commands.AcceptWorkspaceInvite;

public record AcceptWorkspaceInviteCommand(Guid UserId, string UserEmail, string Token);
