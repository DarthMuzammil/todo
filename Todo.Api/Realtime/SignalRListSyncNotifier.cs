using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Todo.Api.Hubs;
using Todo.Application.Realtime;

namespace Todo.Api.Realtime;

public class SignalRListSyncNotifier : IListSyncNotifier
{
    private readonly IHubContext<ListSyncHub> _hubContext;

    public SignalRListSyncNotifier(IHubContext<ListSyncHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyTaskCreatedAsync(TaskSyncDto task, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.Group(ListSyncHub.GroupName(task.ListId))
            .SendAsync("TaskCreated", task, cancellationToken);

    public Task NotifyTaskUpdatedAsync(TaskSyncDto task, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.Group(ListSyncHub.GroupName(task.ListId))
            .SendAsync("TaskUpdated", task, cancellationToken);

    public Task NotifyTaskDeletedAsync(
        Guid taskId,
        Guid listId,
        CancellationToken cancellationToken = default) =>
        _hubContext.Clients.Group(ListSyncHub.GroupName(listId))
            .SendAsync("TaskDeleted", new { taskId, listId }, cancellationToken);

    public Task NotifyListUpdatedAsync(ListSyncDto list, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.Group(ListSyncHub.GroupName(list.Id))
            .SendAsync("ListUpdated", list, cancellationToken);

    public Task NotifyListDeletedAsync(Guid listId, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.Group(ListSyncHub.GroupName(listId))
            .SendAsync("ListDeleted", new { listId }, cancellationToken);
}
