using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Queries.GetListById;

public class GetListByIdHandler
{
    private readonly ListAccessChecker _access;

    public GetListByIdHandler(ListAccessChecker access)
    {
        _access = access;
    }

    public Task<Result<TodoList>> HandleAsync(GetListByIdQuery query) =>
        _access.RequireReadableListAsync(query.ListId, query.UserId);
}
