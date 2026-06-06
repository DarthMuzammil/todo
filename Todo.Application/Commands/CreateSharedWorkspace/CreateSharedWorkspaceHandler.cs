using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Commands.CreateSharedWorkspace;

public class CreateSharedWorkspaceHandler
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSharedWorkspaceHandler(
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork)
    {
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Workspace>> HandleAsync(CreateSharedWorkspaceCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<Workspace>.Failure("Workspace name is required.");

        var trimmedName = command.Name.Trim();
        if (trimmedName.Length > 200)
            return Result<Workspace>.Failure("Workspace name must be at most 200 characters.");

        var createResult = await _workspaceRepository.CreateSharedWorkspaceAsync(
            command.UserId,
            trimmedName);

        if (!createResult.IsSuccess)
            return createResult;

        await _unitOfWork.CommitAsync();
        return createResult;
    }
}
