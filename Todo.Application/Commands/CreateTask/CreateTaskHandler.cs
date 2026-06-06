using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Application.Common;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Application.Commands.CreateTask;

public class CreateTaskHandler
{
    private readonly ITaskRepository _taskRepository;

    public CreateTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<TodoTask>> HandleAsync(CreateTaskCommand command)
    {
        var task = new TodoTask
        {
            Id = Guid.NewGuid(),
            ListId = command.ListId,
            Title = command.Title,
            Description = command.Description ?? string.Empty,
            Status = TaskStatus.Todo,
            Priority = command.Priority,
            DueDate = command.DueDate,
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        // var evt = new TaskCreatedEvent(...) — wire later

        return await _taskRepository.AddAsync(task);
    }
}