using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Todo.Api.Extensions;
using Todo.Application.Common;

namespace Todo.Api.Hubs;

[Authorize]
public class ListSyncHub : Hub
{
    private readonly ListAccessChecker _access;

    public ListSyncHub(ListAccessChecker access)
    {
        _access = access;
    }

    public static string GroupName(Guid listId) => $"list:{listId}";

    public async Task JoinList(Guid listId)
    {
        var userId = Context.User!.GetUserId();
        var result = await _access.RequireReadableListAsync(listId, userId);

        if (!result.IsSuccess)
            throw new HubException(result.Error ?? "Forbidden");

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(listId));
    }

    public Task LeaveList(Guid listId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(listId));
}
