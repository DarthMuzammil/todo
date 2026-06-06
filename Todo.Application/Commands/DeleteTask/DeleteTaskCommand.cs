namespace Todo.Application.Commands.DeleteTask;

public record DeleteTaskCommand(Guid TaskId, Guid UserId);