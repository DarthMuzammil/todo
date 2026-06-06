using Todo.Application.Interfaces;
using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Commands.DeleteTask;

public class DeleteTaskHandler
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<TodoTask>> HandleAsync(DeleteTaskCommand command)
    {
        return await _taskRepository.RemoveAsync(command.TaskId);
    }
}