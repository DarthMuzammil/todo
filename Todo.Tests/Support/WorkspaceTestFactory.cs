using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Repositories;

namespace Todo.Tests.Support;

public static class WorkspaceTestFactory
{
    public static async Task<Guid> SeedPersonalWorkspaceAsync(TodoDbContext context, Guid userId)
    {
        var repository = new EfWorkspaceRepository(context);
        var existing = await repository.GetPersonalWorkspaceByUserIdAsync(userId);
        if (existing.IsSuccess)
            return existing.Value!.Id;

        var result = await repository.CreatePersonalWorkspaceAsync(userId);
        await context.SaveChangesAsync();
        return result.Value!.Id;
    }

    public static async Task<TodoList> SeedListAsync(
        TodoDbContext context,
        Guid ownerId,
        string title = "Test list",
        string color = "#ffffff")
    {
        var workspaceId = await SeedPersonalWorkspaceAsync(context, ownerId);

        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            WorkspaceId = workspaceId,
            Title = title,
            Color = color,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        context.Lists.Add(list);
        await context.SaveChangesAsync();
        return list;
    }

    public static async Task<(Guid WorkspaceId, WorkspaceMember Member)> SeedSharedWorkspaceAsync(
        TodoDbContext context,
        Guid ownerId,
        string name = "Shared")
    {
        var repository = new EfWorkspaceRepository(context);
        var workspaceResult = await repository.CreateSharedWorkspaceAsync(ownerId, name);
        await context.SaveChangesAsync();

        var memberResult = await repository.GetMemberAsync(workspaceResult.Value!.Id, ownerId);
        return (workspaceResult.Value!.Id, memberResult.Value!);
    }

    public static async Task AddMemberAsync(
        TodoDbContext context,
        Guid workspaceId,
        Guid userId,
        WorkspaceRole role)
    {
        var repository = new EfWorkspaceRepository(context);
        await repository.AddMemberAsync(workspaceId, userId, role);
        await context.SaveChangesAsync();
    }

    public static async Task<TodoList> SeedListInWorkspaceAsync(
        TodoDbContext context,
        Guid ownerId,
        Guid workspaceId,
        string title = "Shared list",
        string color = "#ffffff")
    {
        var list = new TodoList
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            WorkspaceId = workspaceId,
            Title = title,
            Color = color,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        context.Lists.Add(list);
        await context.SaveChangesAsync();
        return list;
    }
}
