using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Queries.GetListsByOwnerId;

public class GetListsByOwnerIdHandler
{
    private readonly IListRepository _listRepository;
    private readonly IWorkspaceRepository _workspaceRepository;

    public GetListsByOwnerIdHandler(
        IListRepository listRepository,
        IWorkspaceRepository workspaceRepository)
    {
        _listRepository = listRepository;
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Result<List<TodoList>>> HandleAsync(GetListsByOwnerIdQuery query)
    {
        var workspacesResult = await _workspaceRepository.GetWorkspacesForUserAsync(query.UserId);
        if (!workspacesResult.IsSuccess)
            return Result<List<TodoList>>.Failure(workspacesResult.Error!);

        var workspaceIds = workspacesResult.Value!
            .Select(workspace => workspace.Id)
            .ToList();

        return await _listRepository.GetByWorkspaceIdsAsync(workspaceIds);
    }
}
