using System.Text.Json;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Repositories;

public class JsonTaskRepository : ITaskRepository
{
    private readonly string _filePath;

    public JsonTaskRepository(string filePath)
    {
        _filePath = filePath;
    }
    private async Task SaveTasksAsync(List<TodoTask> tasks)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(tasks);
        await File.WriteAllTextAsync(_filePath, json);
    }
    private async Task<List<TodoTask>> LoadTasksAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<TodoTask>();
        }
        var fileText = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<TodoTask>>(fileText) ?? new List<TodoTask>();
    }
    public async Task<Result<TodoTask>> GetTaskByIdAsync(Guid id)
    {
        var tasks = await LoadTasksAsync();
        var task = tasks.FirstOrDefault(t => t.Id == id && !t.IsDeleted);
        if (task == null)
        {
            return Result<TodoTask>.Failure("Task not found");
        }
        else
        {
            return Result<TodoTask>.Success(task);
        }
    }
    public async Task<Result<List<TodoTask>>> GetByListIdAsync(Guid listId)
    {
        var tasks = await LoadTasksAsync();
        var filtered = tasks.Where(task => task.ListId == listId && !task.IsDeleted).ToList();
        return Result<List<TodoTask>>.Success(filtered);
    }
    public async Task<Result<TodoTask>> AddAsync(TodoTask task)
    {
        var tasks = await LoadTasksAsync();
        tasks.Add(task);
        await SaveTasksAsync(tasks);
        return Result<TodoTask>.Success(task);
    }
    public async Task<Result<TodoTask>> UpdateAsync(TodoTask task)
    {
        var tasks = await LoadTasksAsync();
        var index = tasks.FindIndex(t => t.Id == task.Id);
        if (index == -1)
        {
            return Result<TodoTask>.Failure("Task not found");
        }
        tasks[index] = task;
        await SaveTasksAsync(tasks);
        return Result<TodoTask>.Success(task);
    }
    public async Task<Result<TodoTask>> RemoveAsync(Guid id)
    {
        var tasks = await LoadTasksAsync();
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
        {
            return Result<TodoTask>.Failure("Task not found");
        }
        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;
        await SaveTasksAsync(tasks);
        return Result<TodoTask>.Success(task);
    }
}