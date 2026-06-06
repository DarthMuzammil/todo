using Todo.Domain.Enums;

namespace Todo.Application.Commands.CreateList;

public record CreateListCommand(Guid OwnerId, string Title, string? Color);