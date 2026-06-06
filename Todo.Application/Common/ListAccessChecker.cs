using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Common;

public class ListAccessChecker
{
    public const string NotFoundMessage = "List not found";
    public const string ForbiddenMessage = "Forbidden";

    private readonly IListRepository _listRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IWorkspaceRepository _workspaceRepository;

    public ListAccessChecker(
        IListRepository listRepository,
        ITaskRepository taskRepository,
        IWorkspaceRepository workspaceRepository)
    {
        _listRepository = listRepository;
        _taskRepository = taskRepository;
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Result<TodoList>> RequireReadableListAsync(Guid listId, Guid userId)
    {
        var listResult = await _listRepository.GetByIdAsync(listId);
        if (!listResult.IsSuccess)
            return listResult;

        return await RequireWorkspaceMemberAsync(listResult.Value!, userId);
    }

    public async Task<Result<TodoList>> RequireWritableListAsync(Guid listId, Guid userId)
    {
        var listResult = await RequireReadableListAsync(listId, userId);
        if (!listResult.IsSuccess)
            return listResult;

        var memberResult = await _workspaceRepository.GetMemberAsync(
            listResult.Value!.WorkspaceId,
            userId);

        if (!memberResult.IsSuccess)
            return Result<TodoList>.Failure(NotFoundMessage);

        if (memberResult.Value!.Role == WorkspaceRole.Viewer)
            return Result<TodoList>.Failure(ForbiddenMessage);

        return listResult;
    }

    public async Task<Result<TodoList>> RequireWritableListForTaskAsync(Guid taskId, Guid userId)
    {
        var taskResult = await _taskRepository.GetTaskByIdAsync(taskId);
        if (!taskResult.IsSuccess)
            return Result<TodoList>.Failure(taskResult.Error!);

        return await RequireWritableListAsync(taskResult.Value!.ListId, userId);
    }

    public async Task<Result<Workspace>> RequireWritableWorkspaceAsync(Guid workspaceId, Guid userId)
    {
        var workspaceResult = await _workspaceRepository.GetByIdAsync(workspaceId);
        if (!workspaceResult.IsSuccess)
            return Result<Workspace>.Failure(NotFoundMessage);

        var memberResult = await _workspaceRepository.GetMemberAsync(workspaceId, userId);
        if (!memberResult.IsSuccess)
            return Result<Workspace>.Failure(NotFoundMessage);

        if (memberResult.Value!.Role == WorkspaceRole.Viewer)
            return Result<Workspace>.Failure(ForbiddenMessage);

        return workspaceResult;
    }

    private async Task<Result<TodoList>> RequireWorkspaceMemberAsync(TodoList list, Guid userId)
    {
        var memberResult = await _workspaceRepository.GetMemberAsync(list.WorkspaceId, userId);
        if (!memberResult.IsSuccess)
            return Result<TodoList>.Failure(NotFoundMessage);

        return Result<TodoList>.Success(list);
    }
}
