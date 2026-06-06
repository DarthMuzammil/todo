using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Application.Common;
using Todo.Application.Interfaces;

namespace Todo.Infrastructure.Persistence;

public class WorkspaceDataBackfill
{
    private readonly TodoDbContext _context;
    private readonly PersonalWorkspaceService _personalWorkspaceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WorkspaceDataBackfill> _logger;

    public WorkspaceDataBackfill(
        TodoDbContext context,
        PersonalWorkspaceService personalWorkspaceService,
        IUnitOfWork unitOfWork,
        ILogger<WorkspaceDataBackfill> logger)
    {
        _context = context;
        _personalWorkspaceService = personalWorkspaceService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var ownerIds = await _context.Lists
            .IgnoreQueryFilters()
            .Where(l => l.WorkspaceId == Guid.Empty)
            .Select(l => l.OwnerId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var userIdsWithoutLists = await _context.Users
            .Select(u => u.Id)
            .Where(id => !_context.Workspaces.Any(w => w.IsPersonal && w.PersonalOwnerId == id))
            .ToListAsync(cancellationToken);

        var userIds = ownerIds
            .Concat(userIdsWithoutLists)
            .Distinct()
            .ToList();

        foreach (var userId in userIds)
        {
            var workspaceResult = await _personalWorkspaceService.EnsurePersonalWorkspaceAsync(userId);
            if (!workspaceResult.IsSuccess)
            {
                _logger.LogWarning(
                    "Skipping workspace backfill for user {UserId}: {Error}",
                    userId,
                    workspaceResult.Error);
                continue;
            }

            var workspaceId = workspaceResult.Value!.Id;

            var lists = await _context.Lists
                .IgnoreQueryFilters()
                .Where(l => l.OwnerId == userId && l.WorkspaceId == Guid.Empty)
                .ToListAsync(cancellationToken);

            foreach (var list in lists)
            {
                list.WorkspaceId = workspaceId;
            }

            if (lists.Count > 0)
            {
                await _unitOfWork.CommitAsync();
                _logger.LogInformation(
                    "Backfilled {Count} lists for user {UserId} into workspace {WorkspaceId}",
                    lists.Count,
                    userId,
                    workspaceId);
            }
        }
    }
}