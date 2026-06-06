using Todo.Application.Common;

namespace Todo.Application.Interfaces;

public interface IUnitOfWork
{
    Task<Result> CommitAsync();
}