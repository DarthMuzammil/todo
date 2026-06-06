using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Repositories;

public class EfWorkspaceInviteRepository : IWorkspaceInviteRepository
{
    private readonly TodoDbContext _context;

    public EfWorkspaceInviteRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<Result<WorkspaceInvite>> GetByIdAsync(Guid inviteId)
    {
        var invite = await _context.WorkspaceInvites
            .FirstOrDefaultAsync(i => i.Id == inviteId);

        return invite is null
            ? Result<WorkspaceInvite>.Failure("Invite not found")
            : Result<WorkspaceInvite>.Success(invite);
    }

    public async Task<Result<WorkspaceInvite>> GetByTokenHashAsync(string tokenHash)
    {
        var invite = await _context.WorkspaceInvites
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash);

        return invite is null
            ? Result<WorkspaceInvite>.Failure("Invite not found")
            : Result<WorkspaceInvite>.Success(invite);
    }

    public async Task<Result<WorkspaceInvite>> GetPendingByWorkspaceAndEmailAsync(
        Guid workspaceId,
        string email)
    {
        var invite = await _context.WorkspaceInvites
            .FirstOrDefaultAsync(i =>
                i.WorkspaceId == workspaceId
                && i.Email == email
                && i.Status == WorkspaceInviteStatus.Pending
                && i.ExpiresAt > DateTime.UtcNow);

        return invite is null
            ? Result<WorkspaceInvite>.Failure("Invite not found")
            : Result<WorkspaceInvite>.Success(invite);
    }

    public async Task<Result<List<WorkspaceInvite>>> GetPendingByWorkspaceIdAsync(Guid workspaceId)
    {
        var invites = await _context.WorkspaceInvites
            .Where(i =>
                i.WorkspaceId == workspaceId
                && i.Status == WorkspaceInviteStatus.Pending
                && i.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return Result<List<WorkspaceInvite>>.Success(invites);
    }

    public Task<Result<WorkspaceInvite>> AddAsync(WorkspaceInvite invite)
    {
        _context.WorkspaceInvites.Add(invite);
        return Task.FromResult(Result<WorkspaceInvite>.Success(invite));
    }

    public async Task<Result<WorkspaceInvite>> UpdateAsync(WorkspaceInvite invite)
    {
        var existing = await _context.WorkspaceInvites
            .FirstOrDefaultAsync(i => i.Id == invite.Id);

        if (existing is null)
            return Result<WorkspaceInvite>.Failure("Invite not found");

        _context.Entry(existing).CurrentValues.SetValues(invite);
        return Result<WorkspaceInvite>.Success(existing);
    }
}
