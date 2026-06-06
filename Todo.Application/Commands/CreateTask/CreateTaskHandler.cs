using Todo.Application.Common;
using Todo.Application.Activity;
using Todo.Application.Interfaces;
using Todo.Application.Realtime;
using Todo.Domain.Entities;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Application.Commands.CreateTask;

public class CreateTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ListAccessChecker _access;
    private readonly IListSyncNotifier _notifier;
    private readonly ActivityRecorder _activityRecorder;

    public CreateTaskHandler(
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

    public async Task<Result<TodoTask>> HandleAsync(CreateTaskCommand command)
    {
        var listResult = await _access.RequireWritableListAsync(command.ListId, command.UserId);
        if (!listResult.IsSuccess)
            return Result<TodoTask>.Failure(listResult.Error!);

        var list = listResult.Value!;

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
            IsDeleted = false,
            Version = 1
        };

        var addResult = await _taskRepository.AddAsync(task);
        if (!addResult.IsSuccess)
            return addResult;

        await _activityRecorder.RecordTaskCreatedAsync(task, list, command.UserId);
        await _unitOfWork.CommitAsync();

        if (addResult.IsSuccess)
            await _notifier.NotifyTaskCreatedAsync(RealtimeMapper.ToTaskDto(addResult.Value!));

        return addResult;
    }
}