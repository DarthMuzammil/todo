using Todo.Domain.Enums;

namespace Todo.Application.Commands.CreateTask;

public record CreateTaskCommand(Guid ListId, string Title, string? Description, Priority Priority, DateTime? DueDate);