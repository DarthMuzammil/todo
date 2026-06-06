using Todo.Application.Activity;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Realtime;
using Todo.Domain.Entities;

namespace Todo.Application.Commands.DeleteList;

public class DeleteListHandler
{
    private readonly IListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ListAccessChecker _access;
    private readonly IListSyncNotifier _notifier;
    private readonly ActivityRecorder _activityRecorder;

    public DeleteListHandler(
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

    public async Task<Result<TodoList>> HandleAsync(DeleteListCommand command)
    {
        var listResult = await _access.RequireWritableListAsync(command.ListId, command.UserId);
        if (!listResult.IsSuccess)
            return Result<TodoList>.Failure(listResult.Error!);

        var list = listResult.Value!;

        var removeResult = await _listRepository.RemoveAsync(command.ListId);
        if (!removeResult.IsSuccess)
            return removeResult;

        await _activityRecorder.RecordListDeletedAsync(list, command.UserId);
        await _unitOfWork.CommitAsync();

        await _notifier.NotifyListDeletedAsync(command.ListId);

        return removeResult;
    }
}
