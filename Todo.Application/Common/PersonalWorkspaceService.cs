using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Common;

public class PersonalWorkspaceService
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PersonalWorkspaceService(
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork)
    {
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Workspace>> EnsurePersonalWorkspaceAsync(Guid userId)
    {
        var existing = await _workspaceRepository.GetPersonalWorkspaceByUserIdAsync(userId);
        if (existing.IsSuccess)
            return existing;

        var createResult = await _workspaceRepository.CreatePersonalWorkspaceAsync(userId);
        if (!createResult.IsSuccess)
            return createResult;

        await _unitOfWork.CommitAsync();
        return createResult;
    }
}