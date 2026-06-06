namespace Todo.Domain.Exceptions;

public class TaskNotFoundException : DomainException
{
    public TaskNotFoundException(Guid taskId) : base($"Task with id {taskId} was not found")
    {
        
    }
}
