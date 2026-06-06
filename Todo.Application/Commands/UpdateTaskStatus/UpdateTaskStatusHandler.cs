using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Events;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Application.Commands.UpdateTaskStatus;

public class UpdateTaskStatusHandler
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskStatusHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<TodoTask>> HandleAsync(UpdateTaskStatusCommand command)
    {
        var getResult = await _taskRepository.GetTaskByIdAsync(command.TaskId);
        if (!getResult.IsSuccess)
            return Result<TodoTask>.Failure(getResult.Error!);

        var task = getResult.Value!;
        var oldStatus = task.Status;

        task.Status = command.NewStatus;
        task.UpdatedAt = DateTime.UtcNow;

        _ = new TaskStatusChangedEvent(
            task.Id,
            oldStatus,
            command.NewStatus,
            DateTime.UtcNow);

        return await _taskRepository.UpdateAsync(task);
    }
}
