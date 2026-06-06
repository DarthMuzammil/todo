using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Application.Common;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Application.Commands.CreateTask;

public class CreateTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
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

        var addResult = await _taskRepository.AddAsync(task);
        if (!addResult.IsSuccess)
            return addResult;

        await _unitOfWork.CommitAsync();
        return addResult;
    }
}