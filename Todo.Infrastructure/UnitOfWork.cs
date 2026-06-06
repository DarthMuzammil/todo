using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly TodoDbContext _context;

    public UnitOfWork(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<Result> CommitAsync()
    {
        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
