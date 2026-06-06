using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Queries.GetListById;

public class GetListByIdHandler
{
    private readonly IListRepository _listRepository;

    public GetListByIdHandler(IListRepository listRepository)
    {
        _listRepository = listRepository;
    }

    public async Task<Result<TodoList>> HandleAsync(GetListByIdQuery query)
    {
        return await _listRepository.GetByIdAsync(query.ListId);
    }
}
