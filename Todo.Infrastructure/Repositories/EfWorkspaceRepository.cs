using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Workspaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Repositories;

public class EfWorkspaceRepository : IWorkspaceRepository
{
    private readonly TodoDbContext _context;

    public EfWorkspaceRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Workspace>> GetByIdAsync(Guid workspaceId)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId);

        return workspace is null
            ? Result<Workspace>.Failure("Workspace not found")
            : Result<Workspace>.Success(workspace);
    }

    public async Task<Result<Workspace>> GetPersonalWorkspaceByUserIdAsync(Guid userId)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.IsPersonal && w.PersonalOwnerId == userId);

        return workspace is null
            ? Result<Workspace>.Failure("Personal workspace not found")
            : Result<Workspace>.Success(workspace);
    }

    public async Task<Result<List<Workspace>>> GetWorkspacesForUserAsync(Guid userId)
    {
        var workspaceIds = await _context.WorkspaceMembers
            .Where(m => m.UserId == userId)
            .Select(m => m.WorkspaceId)
            .ToListAsync();

        var workspaces = await _context.Workspaces
            .Where(w => workspaceIds.Contains(w.Id))
            .OrderByDescending(w => w.IsPersonal)
            .ThenByDescending(w => w.UpdatedAt)
            .ToListAsync();

        return Result<List<Workspace>>.Success(workspaces);
    }

    public async Task<Result<WorkspaceMember>> GetMemberAsync(Guid workspaceId, Guid userId)
    {
        var member = await _context.WorkspaceMembers
            .FirstOrDefaultAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId);

        return member is null
            ? Result<WorkspaceMember>.Failure("Member not found")
            : Result<WorkspaceMember>.Success(member);
    }

    public Task<Result<Workspace>> CreatePersonalWorkspaceAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Personal",
            IsPersonal = true,
            PersonalOwnerId = userId,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        _context.Workspaces.Add(workspace);

        var member = new WorkspaceMember
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspace.Id,
            UserId = userId,
            Role = WorkspaceRole.Owner,
            JoinedAt = now
        };

        _context.WorkspaceMembers.Add(member);

        return Task.FromResult(Result<Workspace>.Success(workspace));
    }

    public Task<Result<Workspace>> CreateSharedWorkspaceAsync(Guid userId, string name)
    {
        var now = DateTime.UtcNow;
        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsPersonal = false,
            PersonalOwnerId = null,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        _context.Workspaces.Add(workspace);

        var member = new WorkspaceMember
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspace.Id,
            UserId = userId,
            Role = WorkspaceRole.Owner,
            JoinedAt = now
        };

        _context.WorkspaceMembers.Add(member);

        return Task.FromResult(Result<Workspace>.Success(workspace));
    }

    public Task<Result<WorkspaceMember>> AddMemberAsync(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole role)
    {
        var member = new WorkspaceMember
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };

        _context.WorkspaceMembers.Add(member);
        return Task.FromResult(Result<WorkspaceMember>.Success(member));
    }

    public async Task<bool> IsMemberByEmailAsync(Guid workspaceId, string email)
    {
        return await _context.WorkspaceMembers
            .Join(
                _context.Users,
                member => member.UserId,
                user => user.Id,
                (member, user) => new { member, user })
            .AnyAsync(x =>
                x.member.WorkspaceId == workspaceId
                && x.user.Email != null
                && x.user.Email.ToLower() == email);
    }

    public async Task<Result<List<WorkspaceMemberDetailDto>>> GetMemberDetailsAsync(Guid workspaceId)
    {
        var members = await _context.WorkspaceMembers
            .Where(member => member.WorkspaceId == workspaceId)
            .Join(
                _context.Users,
                member => member.UserId,
                user => user.Id,
                (member, user) => new WorkspaceMemberDetailDto(
                    user.Id,
                    user.Name,
                    user.Email ?? string.Empty,
                    member.Role,
                    member.JoinedAt))
            .OrderByDescending(detail => detail.Role == WorkspaceRole.Owner)
            .ThenBy(detail => detail.Name)
            .ToListAsync();

        return Result<List<WorkspaceMemberDetailDto>>.Success(members);
    }

    public async Task<Result<List<Guid>>> GetMemberUserIdsAsync(Guid workspaceId)
    {
        var userIds = await _context.WorkspaceMembers
            .Where(member => member.WorkspaceId == workspaceId)
            .Select(member => member.UserId)
            .ToListAsync();

        return Result<List<Guid>>.Success(userIds);
    }

    public async Task<Result> RemoveMemberAsync(Guid workspaceId, Guid userId)
    {
        var member = await _context.WorkspaceMembers
            .FirstOrDefaultAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId);

        if (member is null)
            return Result.Failure("Member not found");

        _context.WorkspaceMembers.Remove(member);
        return Result.Success();
    }
}
