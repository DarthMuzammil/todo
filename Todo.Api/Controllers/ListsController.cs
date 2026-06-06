using Microsoft.AspNetCore.Mvc;
using Todo.Application.Commands.CreateList;
using Todo.Application.Queries.GetListById;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/lists")]
public class ListsController : ControllerBase
{
    private readonly CreateListHandler _createListHandler;
    private readonly GetListByIdHandler _getListByIdHandler;

    public ListsController(
        CreateListHandler createListHandler,
        GetListByIdHandler getListByIdHandler)
    {
        _createListHandler = createListHandler;
        _getListByIdHandler = getListByIdHandler;
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
