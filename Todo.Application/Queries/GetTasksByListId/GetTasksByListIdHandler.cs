using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Queries.GetTasksByListId;

public class GetTasksByListIdHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ListAccessChecker _access;

    public GetTasksByListIdHandler(ITaskRepository taskRepository, ListAccessChecker access)
    {
        _taskRepository = taskRepository;
        _access = access;
    }

    public async Task<Result<List<TodoTask>>> HandleAsync(GetTasksByListIdQuery query)
    {
        var listResult = await _access.RequireReadableListAsync(query.ListId, query.UserId);
        if (!listResult.IsSuccess)
            return Result<List<TodoTask>>.Failure(listResult.Error!);

        return await _taskRepository.GetByListIdAsync(query.ListId);
    }
}
