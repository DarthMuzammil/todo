using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Workspaces;

namespace Todo.Application.Queries.GetWorkspaces;

public class GetWorkspacesHandler
{
    private readonly IWorkspaceRepository _workspaceRepository;

    public GetWorkspacesHandler(IWorkspaceRepository workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Result<List<WorkspaceSummaryDto>>> HandleAsync(GetWorkspacesQuery query)
    {
        var workspacesResult = await _workspaceRepository.GetWorkspacesForUserAsync(query.UserId);
        if (!workspacesResult.IsSuccess)
            return Result<List<WorkspaceSummaryDto>>.Failure(workspacesResult.Error!);

        var summaries = new List<WorkspaceSummaryDto>();

        foreach (var workspace in workspacesResult.Value!)
        {
            var memberResult = await _workspaceRepository.GetMemberAsync(workspace.Id, query.UserId);
            if (!memberResult.IsSuccess)
                continue;

            summaries.Add(new WorkspaceSummaryDto(
                workspace.Id,
                workspace.Name,
                workspace.IsPersonal,
                memberResult.Value!.Role));
        }

        return Result<List<WorkspaceSummaryDto>>.Success(summaries);
    }
}
