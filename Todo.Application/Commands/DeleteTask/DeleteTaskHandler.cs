using Todo.Application.Interfaces;
using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Commands.DeleteTask;

public class DeleteTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TodoTask>> HandleAsync(DeleteTaskCommand command)
    {
        var removeResult = await _taskRepository.RemoveAsync(command.TaskId);
        if (!removeResult.IsSuccess)
            return removeResult;

        await _unitOfWork.CommitAsync();
        return removeResult;
    }
}