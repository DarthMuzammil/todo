using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Application.Activity;

public class ActivityRecorder
{
    private readonly IActivityRepository _activityRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUserDirectory _userDirectory;

    public ActivityRecorder(
        IActivityRepository activityRepository,
        INotificationRepository notificationRepository,
        IWorkspaceRepository workspaceRepository,
        IUserDirectory userDirectory)
    {
        _activityRepository = activityRepository;
        _notificationRepository = notificationRepository;
        _workspaceRepository = workspaceRepository;
        _userDirectory = userDirectory;
    }

    public Task RecordTaskCreatedAsync(TodoTask task, TodoList list, Guid actorUserId) =>
        RecordAsync(
            list.WorkspaceId,
            list.Id,
            task.Id,
            actorUserId,
            ActivityAction.TaskCreated,
            task.Title,
            list.Title);

    public Task RecordTaskStatusChangedAsync(
        TodoTask task,
        TodoList list,
        Guid actorUserId,
        TaskStatus previousStatus,
        TaskStatus newStatus) =>
        RecordAsync(
            list.WorkspaceId,
            list.Id,
            task.Id,
            actorUserId,
            ActivityAction.TaskStatusChanged,
            task.Title,
            list.Title,
            previousStatus,
            newStatus);

    public Task RecordTaskDeletedAsync(
        TodoTask task,
        TodoList list,
        Guid actorUserId) =>
        RecordAsync(
            list.WorkspaceId,
            list.Id,
            task.Id,
            actorUserId,
            ActivityAction.TaskDeleted,
            task.Title,
            list.Title);

    public Task RecordListCreatedAsync(TodoList list, Guid actorUserId) =>
        RecordAsync(
            list.WorkspaceId,
            list.Id,
            null,
            actorUserId,
            ActivityAction.ListCreated,
            string.Empty,
            list.Title);

    public Task RecordListUpdatedAsync(TodoList list, Guid actorUserId) =>
        RecordAsync(
            list.WorkspaceId,
            list.Id,
            null,
            actorUserId,
            ActivityAction.ListUpdated,
            string.Empty,
            list.Title);

    public Task RecordListDeletedAsync(TodoList list, Guid actorUserId) =>
        RecordAsync(
            list.WorkspaceId,
            list.Id,
            null,
            actorUserId,
            ActivityAction.ListDeleted,
            string.Empty,
            list.Title);

    private async Task RecordAsync(
        Guid workspaceId,
        Guid listId,
        Guid? taskId,
        Guid actorUserId,
        ActivityAction action,
        string taskTitle,
        string listTitle,
        TaskStatus? previousStatus = null,
        TaskStatus? newStatus = null)
    {
        var actorName = await _userDirectory.GetDisplayNameAsync(actorUserId);
        var now = DateTime.UtcNow;
        var activityId = Guid.NewGuid();

        var activity = new ActivityEntry
        {
            Id = activityId,
            WorkspaceId = workspaceId,
            ListId = listId,
            TaskId = taskId,
            ActorUserId = actorUserId,
            Action = action,
            TaskTitle = taskTitle,
            ListTitle = listTitle,
            PreviousStatus = previousStatus.HasValue ? (int)previousStatus.Value : null,
            NewStatus = newStatus.HasValue ? (int)newStatus.Value : null,
            CreatedAt = now,
        };

        await _activityRepository.AddAsync(activity);

        var membersResult = await _workspaceRepository.GetMemberUserIdsAsync(workspaceId);
        if (!membersResult.IsSuccess)
            return;

        var message = ActivityMessageFormatter.Format(
            action,
            actorName,
            taskTitle,
            listTitle,
            previousStatus,
            newStatus);

        var notifications = membersResult.Value!
            .Where(userId => userId != actorUserId)
            .Select(userId => new UserNotification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ActivityId = activityId,
                ListId = listId,
                TaskId = taskId,
                Message = message,
                IsRead = false,
                CreatedAt = now,
            })
            .ToList();

        if (notifications.Count > 0)
            await _notificationRepository.AddRangeAsync(notifications);
    }
}
