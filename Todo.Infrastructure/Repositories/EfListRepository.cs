using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Repositories;

public class EfListRepository : IListRepository
{
    public readonly TodoDbContext _context;

    public EfListRepository(TodoDbContext context)
    {
        _context = context;
    }
    public async Task<Result<TodoList>> GetByIdAsync(System.Guid id)
    {
        var list = await _context.Lists.FirstOrDefaultAsync(l => l.Id == id);
        if (list is null)
        {
            return Result<TodoList>.Failure("List not found");
        }
        return Result<TodoList>.Success(list);
    }
    public async Task<Result<List<TodoList>>> GetByOwnerIdAsync(System.Guid ownerId)
    {
        var lists = await _context.Lists
            .Where(l => l.OwnerId == ownerId)
            .OrderByDescending(l => l.UpdatedAt)
            .ToListAsync();

        return Result<List<TodoList>>.Success(lists);
    }

    public async Task<Result<List<TodoList>>> GetByWorkspaceIdsAsync(IReadOnlyCollection<System.Guid> workspaceIds)
    {
        if (workspaceIds.Count == 0)
            return Result<List<TodoList>>.Success([]);

        var lists = await _context.Lists
            .Where(l => workspaceIds.Contains(l.WorkspaceId))
            .OrderByDescending(l => l.UpdatedAt)
            .ToListAsync();

        return Result<List<TodoList>>.Success(lists);
    }

    public Task<Result<TodoList>> AddAsync(TodoList list)
    {
        _context.Lists.Add(list);
        return Task.FromResult(Result<TodoList>.Success(list));
    }
    public async Task<Result<TodoList>> UpdateAsync(TodoList list)
    {
        var existing = await _context.Lists
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(l => l.Id == list.Id);

        if (existing is null)
            return Result<TodoList>.Failure("List not found");

        _context.Entry(existing).CurrentValues.SetValues(list);
        return Result<TodoList>.Success(existing);
    }
    public async Task<Result<TodoList>> RemoveAsync(System.Guid id)
    {
        var list = await _context.Lists
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == id);
        if (list is null)
            return Result<TodoList>.Failure("List not found");
        list.IsDeleted = true;
        return Result<TodoList>.Success(list);
    }
}