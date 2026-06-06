using TaskStatus = Todo.Domain.Enums.TaskStatus;
namespace Todo.Application.Commands.UpdateTaskStatus;
public record UpdateTaskStatusCommand(Guid TaskId, TaskStatus NewStatus);