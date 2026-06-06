using Microsoft.AspNetCore.Mvc;
using Todo.Application.Commands.CreateTask;
using Todo.Application.Commands.DeleteTask;
using Todo.Application.Commands.UpdateTaskStatus;
using Todo.Application.Queries.GetTasksByListId;
using Todo.Domain.Enums;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/lists/{listId:guid}/tasks")]
public class TasksController : ControllerBase
{
    private readonly GetTasksByListIdHandler _getTasksHandler;
    private readonly CreateTaskHandler _createTaskHandler;
    private readonly UpdateTaskStatusHandler _updateStatusHandler;
    private readonly DeleteTaskHandler _deleteTaskHandler;

    public TasksController(
        GetTasksByListIdHandler getTasksHandler,
        CreateTaskHandler createTaskHandler,
        UpdateTaskStatusHandler updateStatusHandler,
        DeleteTaskHandler deleteTaskHandler)
    {
        _getTasksHandler = getTasksHandler;
        _createTaskHandler = createTaskHandler;
        _updateStatusHandler = updateStatusHandler;
        _deleteTaskHandler = deleteTaskHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetByListId(Guid listId)
    {
        var result = await _getTasksHandler.HandleAsync(new GetTasksByListIdQuery(listId));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid listId, [FromBody] CreateTaskRequest request)
    {
        var result = await _createTaskHandler.HandleAsync(new CreateTaskCommand(
            listId,
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetByListId), new { listId }, result.Value);
    }

    [HttpPatch("{taskId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid listId, Guid taskId, [FromBody] UpdateTaskStatusRequest request)
    {
        var result = await _updateStatusHandler.HandleAsync(
            new UpdateTaskStatusCommand(taskId, request.NewStatus));

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        if (result.Value!.ListId != listId)
            return NotFound(new { error = "Task not found in this list" });

        return Ok(result.Value);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> Delete(Guid listId, Guid taskId)
    {
        var result = await _deleteTaskHandler.HandleAsync(new DeleteTaskCommand(taskId));

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        if (result.Value!.ListId != listId)
            return NotFound(new { error = "Task not found in this list" });

        return NoContent();
    }
}

public record CreateTaskRequest(
    string Title,
    string? Description,
    Priority Priority,
    DateTime? DueDate);

public record UpdateTaskStatusRequest(TaskStatus NewStatus);
