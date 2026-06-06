using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Application.Common;

namespace Todo.Application.Commands.CreateList;

public class CreateListHandler
{
    private readonly IListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateListHandler(IListRepository listRepository, IUnitOfWork unitOfWork)
    {
        _listRepository = listRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TodoList>> HandleAsync(CreateListCommand command)
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = command.OwnerId,
            Title = command.Title,
            Color = command.Color ?? "#3B82F6",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var addResult = await _listRepository.AddAsync(list);
        if (!addResult.IsSuccess)
            return addResult;

        await _unitOfWork.CommitAsync();
        return addResult;
    }
}