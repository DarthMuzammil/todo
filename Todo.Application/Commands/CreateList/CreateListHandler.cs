using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Application.Common;
using Todo.Application.Activity;

namespace Todo.Application.Commands.CreateList;

public class CreateListHandler
{
    private readonly IListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PersonalWorkspaceService _personalWorkspaceService;
    private readonly ListAccessChecker _access;
    private readonly ActivityRecorder _activityRecorder;

    public CreateListHandler(
        IListRepository listRepository,
        IUnitOfWork unitOfWork,
        PersonalWorkspaceService personalWorkspaceService,
        ListAccessChecker access,
        ActivityRecorder activityRecorder)
    {
        _listRepository = listRepository;
        _unitOfWork = unitOfWork;
        _personalWorkspaceService = personalWorkspaceService;
        _access = access;
        _activityRecorder = activityRecorder;
    }

    public async Task<Result<TodoList>> HandleAsync(CreateListCommand command)
    {
        Guid workspaceId;

        if (command.WorkspaceId.HasValue)
        {
            var writeResult = await _access.RequireWritableWorkspaceAsync(
                command.WorkspaceId.Value,
                command.UserId);

            if (!writeResult.IsSuccess)
                return Result<TodoList>.Failure(writeResult.Error!);

            workspaceId = command.WorkspaceId.Value;
        }
        else
        {
            var workspaceResult = await _personalWorkspaceService.EnsurePersonalWorkspaceAsync(command.UserId);
            if (!workspaceResult.IsSuccess)
                return Result<TodoList>.Failure(workspaceResult.Error!);

            var writeResult = await _access.RequireWritableWorkspaceAsync(
                workspaceResult.Value!.Id,
                command.UserId);

            if (!writeResult.IsSuccess)
                return Result<TodoList>.Failure(writeResult.Error!);

            workspaceId = workspaceResult.Value.Id;
        }

        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = command.UserId,
            WorkspaceId = workspaceId,
            Title = command.Title,
            Color = command.Color ?? "#3B82F6",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var addResult = await _listRepository.AddAsync(list);
        if (!addResult.IsSuccess)
            return addResult;

        await _activityRecorder.RecordListCreatedAsync(list, command.UserId);
        await _unitOfWork.CommitAsync();
        return addResult;
    }
}
