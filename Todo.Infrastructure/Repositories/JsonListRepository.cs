using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using System.Text.Json;
using System.IO;
using Todo.Application.Common;

namespace Todo.Infrastructure.Repositories;

public class JsonListRepository : IListRepository
{
    private readonly string _filePath;

    public JsonListRepository(string filePath)
    {
        _filePath = filePath;
    }
    private async Task SaveListsAsync(List<TodoList> lists)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(lists);
        await File.WriteAllTextAsync(_filePath, json);
    }
    private async Task<List<TodoList>> LoadListsAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<TodoList>();
        }
        var fileText = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<TodoList>>(fileText) ?? new List<TodoList>();
    }
    public async Task<Result<TodoList>> GetByIdAsync(Guid id)
    {
        var lists = await LoadListsAsync();
        var list = lists.FirstOrDefault(t => t.Id == id && !t.IsDeleted);
        if (list == null)
        {
            return Result<TodoList>.Failure("List not found");
        }
        else
        {
            return Result<TodoList>.Success(list);
        }
    }
    public async Task<Result<List<TodoList>>> GetByOwnerIdAsync(Guid ownerId)
    {
        var lists = await LoadListsAsync();
        var filtered = lists
            .Where(l => l.OwnerId == ownerId && !l.IsDeleted)
            .OrderByDescending(l => l.UpdatedAt)
            .ToList();
        return Result<List<TodoList>>.Success(filtered);
    }

    public async Task<Result<List<TodoList>>> GetByWorkspaceIdsAsync(IReadOnlyCollection<Guid> workspaceIds)
    {
        if (workspaceIds.Count == 0)
            return Result<List<TodoList>>.Success([]);

        var lists = await LoadListsAsync();
        var workspaceIdSet = workspaceIds.ToHashSet();
        var filtered = lists
            .Where(l => workspaceIdSet.Contains(l.WorkspaceId) && !l.IsDeleted)
            .OrderByDescending(l => l.UpdatedAt)
            .ToList();

        return Result<List<TodoList>>.Success(filtered);
    }

    public async Task<Result<TodoList>> AddAsync(TodoList list)
    {
        var lists = await LoadListsAsync();
        lists.Add(list);
        await SaveListsAsync(lists);
        return Result<TodoList>.Success(list);
    }
    public async Task<Result<TodoList>> UpdateAsync(TodoList list)
    {
        var lists = await LoadListsAsync();
        var index = lists.FindIndex(t => t.Id == list.Id);
        if (index == -1)
        {
            return Result<TodoList>.Failure("List not found");
        }
        lists[index] = list;
        await SaveListsAsync(lists);
        return Result<TodoList>.Success(list);
    }
    public async Task<Result<TodoList>> RemoveAsync(Guid id)
    {
        var lists = await LoadListsAsync();
        var list = lists.FirstOrDefault(t => t.Id == id);
        if (list == null)
        {
            return Result<TodoList>.Failure("List not found");
        }
        list.IsDeleted = true;
        await SaveListsAsync(lists);
        return Result<TodoList>.Success(list);
    }
}