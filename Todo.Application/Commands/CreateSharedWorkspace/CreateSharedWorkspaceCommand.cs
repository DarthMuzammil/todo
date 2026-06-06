namespace Todo.Application.Commands.CreateSharedWorkspace;

public record CreateSharedWorkspaceCommand(Guid UserId, string Name);
