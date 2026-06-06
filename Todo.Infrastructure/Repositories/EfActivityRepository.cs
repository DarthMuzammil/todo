using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Repositories;

public class EfActivityRepository : IActivityRepository
{
    private readonly TodoDbContext _context;

    public EfActivityRepository(TodoDbContext context)
    {
        _context = context;
    }

    public Task<Result<ActivityEntry>> AddAsync(ActivityEntry activity)
    {
        _context.ActivityEntries.Add(activity);
        return Task.FromResult(Result<ActivityEntry>.Success(activity));
    }

    public async Task<Result<List<ActivityEntry>>> GetByListIdAsync(Guid listId, int limit = 50)
    {
        var entries = await _context.ActivityEntries
            .Where(entry => entry.ListId == listId)
            .OrderByDescending(entry => entry.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return Result<List<ActivityEntry>>.Success(entries);
    }
}
