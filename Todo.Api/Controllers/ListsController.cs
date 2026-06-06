using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Todo.Api.Extensions;

using Todo.Application.Commands.CreateList;

using Todo.Application.Commands.DeleteList;

using Todo.Application.Commands.UpdateList;

using Todo.Application.Queries.GetListById;

using Todo.Application.Queries.GetListActivity;

using Todo.Application.Queries.GetListsByOwnerId;



namespace Todo.Api.Controllers;



[ApiController]

[Route("api/lists")]

[Authorize]

public class ListsController : ControllerBase

{

    private readonly CreateListHandler _createListHandler;

    private readonly GetListByIdHandler _getListByIdHandler;

    private readonly GetListsByOwnerIdHandler _getListsByOwnerIdHandler;

    private readonly UpdateListHandler _updateListHandler;

    private readonly DeleteListHandler _deleteListHandler;

    private readonly GetListActivityHandler _getListActivityHandler;



    public ListsController(

        CreateListHandler createListHandler,

        GetListByIdHandler getListByIdHandler,

        GetListsByOwnerIdHandler getListsByOwnerIdHandler,

        UpdateListHandler updateListHandler,

        DeleteListHandler deleteListHandler,

        GetListActivityHandler getListActivityHandler)

    {

        _createListHandler = createListHandler;

        _getListByIdHandler = getListByIdHandler;

        _getListsByOwnerIdHandler = getListsByOwnerIdHandler;

        _updateListHandler = updateListHandler;

        _deleteListHandler = deleteListHandler;

        _getListActivityHandler = getListActivityHandler;

    }



    [HttpGet]

    public async Task<IActionResult> GetAll()

    {

        var result = await _getListsByOwnerIdHandler.HandleAsync(

            new GetListsByOwnerIdQuery(User.GetUserId()));



        if (!result.IsSuccess)

            return BadRequest(new { error = result.Error });



        return Ok(result.Value);

    }



    [HttpGet("{id:guid}")]

    public async Task<IActionResult> GetById(Guid id)

    {

        var result = await _getListByIdHandler.HandleAsync(new GetListByIdQuery(id, User.GetUserId()));



        if (!result.IsSuccess)

            return result.ToListReadResult(this);



        return Ok(result.Value);

    }



    [HttpGet("{id:guid}/activity")]

    public async Task<IActionResult> GetActivity(Guid id)

    {

        var result = await _getListActivityHandler.HandleAsync(

            new GetListActivityQuery(id, User.GetUserId()));



        if (!result.IsSuccess)

            return result.ToListReadResult(this);



        return Ok(result.Value);

    }



    [HttpPost]

    public async Task<IActionResult> Create([FromBody] CreateListRequest request)

    {

        var result = await _createListHandler.HandleAsync(new CreateListCommand(
            User.GetUserId(),
            request.Title,
            request.Color,
            request.WorkspaceId));



        if (!result.IsSuccess)

            return result.ToListMutationResult(this);



        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);

    }



    [HttpPatch("{id:guid}")]

    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateListRequest request)

    {

        var result = await _updateListHandler.HandleAsync(

            new UpdateListCommand(id, User.GetUserId(), request.Title, request.Color));



        if (!result.IsSuccess)

            return result.ToListMutationResult(this);



        return Ok(result.Value);

    }



    [HttpDelete("{id:guid}")]

    public async Task<IActionResult> Delete(Guid id)

    {

        var result = await _deleteListHandler.HandleAsync(

            new DeleteListCommand(id, User.GetUserId()));



        if (!result.IsSuccess)

            return result.ToListMutationResult(this);



        return NoContent();

    }

}



public record CreateListRequest(string Title, string? Color, Guid? WorkspaceId = null);

public record UpdateListRequest(string? Title, string? Color);


