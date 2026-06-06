namespace Todo.Domain.Events;

public record TaskCreatedEvent(Guid TaskId, DateTime CreatedAt);
