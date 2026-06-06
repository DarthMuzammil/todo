namespace Todo.Application.Commands.DeleteList;

public record DeleteListCommand(Guid ListId, Guid UserId);
