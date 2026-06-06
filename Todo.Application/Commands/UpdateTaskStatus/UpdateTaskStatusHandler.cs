using Todo.Application.Common;
using Todo.Application.Activity;
using Todo.Application.Interfaces;
using Todo.Application.Realtime;
using Todo.Domain.Entities;
using Todo.Domain.Events;

namespace Todo.Application.Commands.UpdateTaskStatus;

public class UpdateTaskStatusHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ListAccessChecker _access;
    private readonly IListSyncNotifier _notifier;
    private readonly ActivityRecorder _activityRecorder;

    public UpdateTaskStatusHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        ListAccessChecker access,
        IListSyncNotifier notifier,
        ActivityRecorder activityRecorder)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _access = access;
        _notifier = notifier;
        _activityRecorder = activityRecorder;
    }

    public async Task<Result<TodoTask>> HandleAsync(UpdateTaskStatusCommand command)
    {
        var listResult = await _access.RequireWritableListForTaskAsync(command.TaskId, command.UserId);
        if (!listResult.IsSuccess)
            return Result<TodoTask>.Failure(listResult.Error!);

        var list = listResult.Value!;

        var getResult = await _taskRepository.GetTaskByIdAsync(command.TaskId);
        if (!getResult.IsSuccess)
            return Result<TodoTask>.Failure(getResult.Error!);

        var task = getResult.Value!;
        var oldStatus = task.Status;

        task.Status = command.NewStatus;
        task.UpdatedAt = DateTime.UtcNow;
        task.Version += 1;

        _ = new TaskStatusChangedEvent(
            task.Id,
            oldStatus,
            command.NewStatus,
            DateTime.UtcNow);

        var updateResult = await _taskRepository.UpdateAsync(task);
        if (!updateResult.IsSuccess)
            return updateResult;

        await _activityRecorder.RecordTaskStatusChangedAsync(
            task,
            list,
            command.UserId,
            oldStatus,
            command.NewStatus);
        await _unitOfWork.CommitAsync();

        if (updateResult.IsSuccess)
            await _notifier.NotifyTaskUpdatedAsync(RealtimeMapper.ToTaskDto(updateResult.Value!));

        return updateResult;
    }
}
