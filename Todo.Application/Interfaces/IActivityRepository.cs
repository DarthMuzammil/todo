using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Interfaces;

public interface IActivityRepository
{
    Task<Result<ActivityEntry>> AddAsync(ActivityEntry activity);
    Task<Result<List<ActivityEntry>>> GetByListIdAsync(Guid listId, int limit = 50);
}
