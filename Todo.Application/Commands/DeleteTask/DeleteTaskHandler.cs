using Todo.Application.Activity;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Realtime;
using Todo.Domain.Entities;

namespace Todo.Application.Commands.DeleteTask;

public class DeleteTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ListAccessChecker _access;
    private readonly IListSyncNotifier _notifier;
    private readonly ActivityRecorder _activityRecorder;

    public DeleteTaskHandler(
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

    public async Task<Result<TodoTask>> HandleAsync(DeleteTaskCommand command)
    {
        var listResult = await _access.RequireWritableListForTaskAsync(command.TaskId, command.UserId);
        if (!listResult.IsSuccess)
            return Result<TodoTask>.Failure(listResult.Error!);

        var list = listResult.Value!;

        var getResult = await _taskRepository.GetTaskByIdAsync(command.TaskId);
        if (!getResult.IsSuccess)
            return Result<TodoTask>.Failure(getResult.Error!);

        var task = getResult.Value!;

        var removeResult = await _taskRepository.RemoveAsync(command.TaskId);
        if (!removeResult.IsSuccess)
            return removeResult;

        await _activityRecorder.RecordTaskDeletedAsync(task, list, command.UserId);
        await _unitOfWork.CommitAsync();

        await _notifier.NotifyTaskDeletedAsync(command.TaskId, list.Id);

        return removeResult;
    }
}
