using Todo.Application.Activity;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Realtime;
using Todo.Domain.Entities;

namespace Todo.Application.Commands.UpdateList;

public class UpdateListHandler
{
    private readonly IListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ListAccessChecker _access;
    private readonly IListSyncNotifier _notifier;
    private readonly ActivityRecorder _activityRecorder;

    public UpdateListHandler(
        IListRepository listRepository,
        IUnitOfWork unitOfWork,
        ListAccessChecker access,
        IListSyncNotifier notifier,
        ActivityRecorder activityRecorder)
    {
        _listRepository = listRepository;
        _unitOfWork = unitOfWork;
        _access = access;
        _notifier = notifier;
        _activityRecorder = activityRecorder;
    }

    public async Task<Result<TodoList>> HandleAsync(UpdateListCommand command)
    {
        var listResult = await _access.RequireWritableListAsync(command.ListId, command.UserId);
        if (!listResult.IsSuccess)
            return Result<TodoList>.Failure(listResult.Error!);

        var list = listResult.Value!;

        var hasTitle = !string.IsNullOrWhiteSpace(command.Title);
        var hasColor = !string.IsNullOrWhiteSpace(command.Color);
        if (!hasTitle && !hasColor)
            return Result<TodoList>.Failure("At least one of title or color must be provided.");

        if (hasTitle)
            list.Title = command.Title!;
        if (hasColor)
            list.Color = command.Color!;

        list.UpdatedAt = DateTime.UtcNow;
        list.Version += 1;

        var updateResult = await _listRepository.UpdateAsync(list);
        if (!updateResult.IsSuccess)
            return updateResult;

        await _activityRecorder.RecordListUpdatedAsync(list, command.UserId);
        await _unitOfWork.CommitAsync();

        if (updateResult.IsSuccess)
            await _notifier.NotifyListUpdatedAsync(RealtimeMapper.ToListDto(updateResult.Value!));

        return updateResult;
    }
}
