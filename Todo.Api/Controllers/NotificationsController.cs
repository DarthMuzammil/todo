using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Extensions;
using Todo.Application.Commands.MarkAllNotificationsRead;
using Todo.Application.Commands.MarkNotificationRead;
using Todo.Application.Queries.GetNotifications;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly GetNotificationsHandler _getNotificationsHandler;
    private readonly MarkNotificationReadHandler _markReadHandler;
    private readonly MarkAllNotificationsReadHandler _markAllReadHandler;

    public NotificationsController(
        GetNotificationsHandler getNotificationsHandler,
        MarkNotificationReadHandler markReadHandler,
        MarkAllNotificationsReadHandler markAllReadHandler)
    {
        _getNotificationsHandler = getNotificationsHandler;
        _markReadHandler = markReadHandler;
        _markAllReadHandler = markAllReadHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int limit = 20)
    {
        var result = await _getNotificationsHandler.HandleAsync(
            new GetNotificationsQuery(User.GetUserId(), limit));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var result = await _getNotificationsHandler.HandleAsync(
            new GetNotificationsQuery(User.GetUserId(), 0));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { unreadCount = result.Value!.UnreadCount });
    }

    [HttpPatch("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid notificationId)
    {
        var result = await _markReadHandler.HandleAsync(
            new MarkNotificationReadCommand(notificationId, User.GetUserId()));

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return NoContent();
    }

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllRead()
    {
        var result = await _markAllReadHandler.HandleAsync(
            new MarkAllNotificationsReadCommand(User.GetUserId()));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
}
