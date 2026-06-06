using Todo.Application.Activity;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Notifications;
using Todo.Domain.Enums;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Application.Queries.GetListActivity;

public class GetListActivityHandler
{
    private readonly IActivityRepository _activityRepository;
    private readonly IUserDirectory _userDirectory;
    private readonly ListAccessChecker _access;

    public GetListActivityHandler(
        IActivityRepository activityRepository,
        IUserDirectory userDirectory,
        ListAccessChecker access)
    {
        _activityRepository = activityRepository;
        _userDirectory = userDirectory;
        _access = access;
    }

    public async Task<Result<List<ActivityItemDto>>> HandleAsync(GetListActivityQuery query)
    {
        var listResult = await _access.RequireReadableListAsync(query.ListId, query.UserId);
        if (!listResult.IsSuccess)
            return Result<List<ActivityItemDto>>.Failure(listResult.Error!);

        var entriesResult = await _activityRepository.GetByListIdAsync(query.ListId);
        if (!entriesResult.IsSuccess)
            return Result<List<ActivityItemDto>>.Failure(entriesResult.Error!);

        var actorNames = new Dictionary<Guid, string>();
        var items = new List<ActivityItemDto>();

        foreach (var entry in entriesResult.Value!)
        {
            if (!actorNames.TryGetValue(entry.ActorUserId, out var actorName))
            {
                actorName = await _userDirectory.GetDisplayNameAsync(entry.ActorUserId);
                actorNames[entry.ActorUserId] = actorName;
            }

            TaskStatus? previousStatus = entry.PreviousStatus.HasValue
                ? (TaskStatus)entry.PreviousStatus.Value
                : null;
            TaskStatus? newStatus = entry.NewStatus.HasValue
                ? (TaskStatus)entry.NewStatus.Value
                : null;

            items.Add(new ActivityItemDto(
                entry.Id,
                actorName,
                ActivityMessageFormatter.Format(
                    entry.Action,
                    actorName,
                    entry.TaskTitle,
                    entry.ListTitle,
                    previousStatus,
                    newStatus),
                entry.CreatedAt));
        }

        return Result<List<ActivityItemDto>>.Success(items);
    }
}
