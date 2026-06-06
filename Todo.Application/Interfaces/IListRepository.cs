using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Interfaces;

public interface IListRepository
{
    Task<Result<TodoList>> GetByIdAsync(Guid id);
    Task<Result<List<TodoList>>> GetByOwnerIdAsync(Guid ownerId);
    Task<Result<TodoList>> AddAsync(TodoList list);
    Task<Result<TodoList>> UpdateAsync(TodoList list);
    Task<Result<TodoList>> RemoveAsync(Guid id);
}