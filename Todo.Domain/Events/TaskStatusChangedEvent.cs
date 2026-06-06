using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Domain.Events;

public record TaskStatusChangedEvent(
    Guid TaskId,
    TaskStatus OldStatus,
    TaskStatus NewStatus,
    DateTime ChangedAt);