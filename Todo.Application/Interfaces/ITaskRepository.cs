using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Interfaces;

public interface ITaskRepository
{
    Task<Result<TodoTask>> GetTaskByIdAsync(Guid id);
    Task<Result<List<TodoTask>>> GetByListIdAsync(Guid listId);
    Task<Result<TodoTask>> AddAsync(TodoTask task);
    Task<Result<TodoTask>> UpdateAsync(TodoTask task);
    Task<Result<TodoTask>> RemoveAsync(Guid id);
}