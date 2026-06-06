using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Interfaces;

public interface IListRepository
{
    Task<Result<TodoList>> GetByIdAsync(System.Guid id);
    Task<Result<List<TodoList>>> GetByOwnerIdAsync(System.Guid ownerId);
    Task<Result<List<TodoList>>> GetByWorkspaceIdsAsync(IReadOnlyCollection<System.Guid> workspaceIds);
    Task<Result<TodoList>> AddAsync(TodoList list);
    Task<Result<TodoList>> UpdateAsync(TodoList list);
    Task<Result<TodoList>> RemoveAsync(System.Guid id);
}