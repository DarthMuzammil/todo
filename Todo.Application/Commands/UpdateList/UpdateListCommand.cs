namespace Todo.Application.Commands.UpdateList;

public record UpdateListCommand(
    Guid ListId,
    Guid UserId,
    string? Title,
    string? Color);
