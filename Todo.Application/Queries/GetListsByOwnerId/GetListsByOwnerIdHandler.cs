using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Queries.GetListsByOwnerId;

public class GetListsByOwnerIdHandler
{
    private readonly IListRepository _listRepository;

    public GetListsByOwnerIdHandler(IListRepository listRepository)
    {
        _listRepository = listRepository;
    }

    public async Task<Result<List<TodoList>>> HandleAsync(GetListsByOwnerIdQuery query)
    {
        return await _listRepository.GetByOwnerIdAsync(query.OwnerId);
    }
}
