namespace Todo.Application.Realtime;

public interface IListSyncNotifier
{
    Task NotifyTaskCreatedAsync(TaskSyncDto task, CancellationToken cancellationToken = default);
    Task NotifyTaskUpdatedAsync(TaskSyncDto task, CancellationToken cancellationToken = default);
    Task NotifyTaskDeletedAsync(Guid taskId, Guid listId, CancellationToken cancellationToken = default);
    Task NotifyListUpdatedAsync(ListSyncDto list, CancellationToken cancellationToken = default);
    Task NotifyListDeletedAsync(Guid listId, CancellationToken cancellationToken = default);
}

public record TaskSyncDto(
    Guid Id,
    Guid ListId,
    string Title,
    string Description,
    int Status,
    int Priority,
    DateTime? DueDate,
    int SortOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    long Version);

public record ListSyncDto(
    Guid Id,
    string Title,
    string Color,
    DateTime UpdatedAt,
    long Version);

public sealed class NullListSyncNotifier : IListSyncNotifier
{
    public Task NotifyTaskCreatedAsync(TaskSyncDto task, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task NotifyTaskUpdatedAsync(TaskSyncDto task, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task NotifyTaskDeletedAsync(Guid taskId, Guid listId, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task NotifyListUpdatedAsync(ListSyncDto list, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task NotifyListDeletedAsync(Guid listId, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}

public static class RealtimeMapper
{
    public static TaskSyncDto ToTaskDto(Domain.Entities.TodoTask task) =>
        new(
            task.Id,
            task.ListId,
            task.Title,
            task.Description,
            (int)task.Status,
            (int)task.Priority,
            task.DueDate,
            task.SortOrder,
            task.CreatedAt,
            task.UpdatedAt,
            task.Version);

    public static ListSyncDto ToListDto(Domain.Entities.TodoList list) =>
        new(list.Id, list.Title, list.Color, list.UpdatedAt, list.Version);
}
