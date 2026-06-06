using Todo.Domain.Enums;

namespace Todo.Application.Commands.CreateList;

public record CreateListCommand(Guid UserId, string Title, string? Color, Guid? WorkspaceId = null);