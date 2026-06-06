using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Repositories;

public class EfTaskRepository : ITaskRepository
{
    private readonly TodoDbContext _context;

    public EfTaskRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TodoTask>> GetTaskByIdAsync(Guid id)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
            return Result<TodoTask>.Failure("Task not found");

        return Result<TodoTask>.Success(task);
    }

    public async Task<Result<List<TodoTask>>> GetByListIdAsync(Guid listId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.ListId == listId)
            .ToListAsync();

        return Result<List<TodoTask>>.Success(tasks);
    }

    public Task<Result<TodoTask>> AddAsync(TodoTask task)
    {
        _context.Tasks.Add(task);
        return Task.FromResult(Result<TodoTask>.Success(task));
    }

    public async Task<Result<TodoTask>> UpdateAsync(TodoTask task)
    {
        var existing = await _context.Tasks
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        if (existing is null)
            return Result<TodoTask>.Failure("Task not found");

        _context.Entry(existing).CurrentValues.SetValues(task);
        return Result<TodoTask>.Success(existing);
    }

    public async Task<Result<TodoTask>> RemoveAsync(Guid id)
    {
        var task = await _context.Tasks
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
            return Result<TodoTask>.Failure("Task not found");

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;
        return Result<TodoTask>.Success(task);
    }
}