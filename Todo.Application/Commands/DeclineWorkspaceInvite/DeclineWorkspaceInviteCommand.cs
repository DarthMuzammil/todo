namespace Todo.Application.Commands.DeclineWorkspaceInvite;

public record DeclineWorkspaceInviteCommand(string UserEmail, string Token);
