using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Application.Common;

namespace Todo.Application.Queries.GetTasksByListId;

public class GetTasksByListIdHandler
{
    private readonly ITaskRepository _taskRepository;

    public GetTasksByListIdHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<List<TodoTask>>> HandleAsync(GetTasksByListIdQuery query)
    {
        return await _taskRepository.GetByListIdAsync(query.ListId);
    }
}