using Microsoft.AspNetCore.Mvc;
using Todo.Application.Commands.CreateList;
using Todo.Application.Queries.GetListById;
using Todo.Application.Queries.GetListsByOwnerId;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/lists")]
public class ListsController : ControllerBase
{
    private readonly CreateListHandler _createListHandler;
    private readonly GetListByIdHandler _getListByIdHandler;
    private readonly GetListsByOwnerIdHandler _getListsByOwnerIdHandler;

    public ListsController(
        CreateListHandler createListHandler,
        GetListByIdHandler getListByIdHandler,
        GetListsByOwnerIdHandler getListsByOwnerIdHandler)
    {
        _createListHandler = createListHandler;
        _getListByIdHandler = getListByIdHandler;
        _getListsByOwnerIdHandler = getListsByOwnerIdHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid ownerId)
    {
        if (ownerId == Guid.Empty)
            return BadRequest(new { error = "ownerId is required" });

        var result = await _getListsByOwnerIdHandler.HandleAsync(new GetListsByOwnerIdQuery(ownerId));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _getListByIdHandler.HandleAsync(new GetListByIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListRequest request)
    {
        var result = await _createListHandler.HandleAsync(new CreateListCommand(
            request.OwnerId,
            request.Title,
            request.Color));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}

public record CreateListRequest(Guid OwnerId, string Title, string? Color);
