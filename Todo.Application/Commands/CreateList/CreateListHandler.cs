using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Application.Common;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Application.Commands.CreateList;

public class CreateListHandler
{
    private readonly IListRepository _listRepository;

    public CreateListHandler(IListRepository listRepository)
    {
        _listRepository = listRepository;
    }

    public async Task<Result<TodoList>> HandleAsync(CreateListCommand command)
    {
        var task = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = command.OwnerId,
            Title = command.Title,
            CreatedAt = DateTime.UtcNow,
            Color = command.Color,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        // var evt = new TaskCreatedEvent(...) — wire later

        return await _listRepository.AddAsync(task);
    }
}