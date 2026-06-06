using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Extensions;
using Todo.Application.Commands.AcceptWorkspaceInvite;
using Todo.Application.Commands.CreateSharedWorkspace;
using Todo.Application.Commands.DeclineWorkspaceInvite;
using Todo.Application.Commands.SendWorkspaceInvite;
using Todo.Application.Common;
using Todo.Application.Queries.GetWorkspaceInvites;
using Todo.Application.Queries.GetWorkspaceMembers;
using Todo.Application.Commands.RemoveWorkspaceMember;
using Todo.Application.Commands.ResendWorkspaceInvite;
using Todo.Application.Queries.GetWorkspaces;
using Todo.Domain.Enums;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/workspaces")]
[Authorize]
public class WorkspacesController : ControllerBase
{
    private readonly CreateSharedWorkspaceHandler _createSharedWorkspaceHandler;
    private readonly GetWorkspacesHandler _getWorkspacesHandler;
    private readonly SendWorkspaceInviteHandler _sendInviteHandler;
    private readonly GetWorkspaceInvitesHandler _getInvitesHandler;
    private readonly GetWorkspaceMembersHandler _getMembersHandler;
    private readonly RemoveWorkspaceMemberHandler _removeMemberHandler;
    private readonly ResendWorkspaceInviteHandler _resendInviteHandler;

    public WorkspacesController(
        CreateSharedWorkspaceHandler createSharedWorkspaceHandler,
        GetWorkspacesHandler getWorkspacesHandler,
        SendWorkspaceInviteHandler sendInviteHandler,
        GetWorkspaceInvitesHandler getInvitesHandler,
        GetWorkspaceMembersHandler getMembersHandler,
        RemoveWorkspaceMemberHandler removeMemberHandler,
        ResendWorkspaceInviteHandler resendInviteHandler)
    {
        _createSharedWorkspaceHandler = createSharedWorkspaceHandler;
        _getWorkspacesHandler = getWorkspacesHandler;
        _sendInviteHandler = sendInviteHandler;
        _getInvitesHandler = getInvitesHandler;
        _getMembersHandler = getMembersHandler;
        _removeMemberHandler = removeMemberHandler;
        _resendInviteHandler = resendInviteHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _getWorkspacesHandler.HandleAsync(
            new GetWorkspacesQuery(User.GetUserId()));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkspaceRequest request)
    {
        var result = await _createSharedWorkspaceHandler.HandleAsync(
            new CreateSharedWorkspaceCommand(User.GetUserId(), request.Name));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        var workspace = result.Value!;
        return CreatedAtAction(
            nameof(GetAll),
            new WorkspaceResponse(workspace.Id, workspace.Name, workspace.IsPersonal));
    }

    [HttpPost("{workspaceId:guid}/invites")]
    public async Task<IActionResult> SendInvite(
        Guid workspaceId,
        [FromBody] SendWorkspaceInviteRequest request)
    {
        if (!Enum.IsDefined(typeof(WorkspaceRole), request.Role))
            return BadRequest(new { error = "Invalid role." });

        var result = await _sendInviteHandler.HandleAsync(
            new SendWorkspaceInviteCommand(
                workspaceId,
                User.GetUserId(),
                request.Email,
                request.Role));

        if (!result.IsSuccess)
        {
            return result.Error == WorkspaceMembershipChecker.NotMemberMessage
                || result.Error == WorkspaceMembershipChecker.WorkspaceNotFoundMessage
                    ? NotFound(new { error = result.Error })
                    : BadRequest(new { error = result.Error });
        }

        var invite = result.Value!;
        return Ok(new SendWorkspaceInviteResponse(
            invite.Id,
            invite.WorkspaceId,
            invite.Email,
            invite.Role,
            invite.Token,
            invite.ExpiresAt));
    }

    [HttpGet("{workspaceId:guid}/invites")]
    public async Task<IActionResult> GetInvites(Guid workspaceId)
    {
        var result = await _getInvitesHandler.HandleAsync(
            new GetWorkspaceInvitesQuery(workspaceId, User.GetUserId()));

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost("{workspaceId:guid}/invites/{inviteId:guid}/resend")]
    public async Task<IActionResult> ResendInvite(Guid workspaceId, Guid inviteId)
    {
        var result = await _resendInviteHandler.HandleAsync(
            new ResendWorkspaceInviteCommand(workspaceId, User.GetUserId(), inviteId));

        if (!result.IsSuccess)
        {
            return result.Error == WorkspaceMembershipChecker.NotMemberMessage
                || result.Error == "Invite not found"
                    ? NotFound(new { error = result.Error })
                    : BadRequest(new { error = result.Error });
        }

        var invite = result.Value!;
        return Ok(new SendWorkspaceInviteResponse(
            invite.Id,
            invite.WorkspaceId,
            invite.Email,
            invite.Role,
            invite.Token,
            invite.ExpiresAt));
    }

    [HttpGet("{workspaceId:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid workspaceId)
    {
        var result = await _getMembersHandler.HandleAsync(
            new GetWorkspaceMembersQuery(workspaceId, User.GetUserId()));

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{workspaceId:guid}/members/{memberUserId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid workspaceId, Guid memberUserId)
    {
        var result = await _removeMemberHandler.HandleAsync(
            new RemoveWorkspaceMemberCommand(workspaceId, User.GetUserId(), memberUserId));

        if (!result.IsSuccess)
        {
            return result.Error == WorkspaceMembershipChecker.NotMemberMessage
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return NoContent();
    }
}

[ApiController]
[Route("api/invites")]
[Authorize]
public class InvitesController : ControllerBase
{
    private readonly AcceptWorkspaceInviteHandler _acceptHandler;
    private readonly DeclineWorkspaceInviteHandler _declineHandler;

    public InvitesController(
        AcceptWorkspaceInviteHandler acceptHandler,
        DeclineWorkspaceInviteHandler declineHandler)
    {
        _acceptHandler = acceptHandler;
        _declineHandler = declineHandler;
    }

    [HttpPost("{token}/accept")]
    public async Task<IActionResult> Accept(string token)
    {
        var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { error = "Email claim is required." });

        var result = await _acceptHandler.HandleAsync(
            new AcceptWorkspaceInviteCommand(User.GetUserId(), email, token));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        var member = result.Value!;
        return Ok(new AcceptInviteResponse(member.WorkspaceId, member.Role));
    }

    [HttpPost("{token}/decline")]
    public async Task<IActionResult> Decline(string token)
    {
        var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { error = "Email claim is required." });

        var result = await _declineHandler.HandleAsync(
            new DeclineWorkspaceInviteCommand(email, token));

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
}

public record CreateWorkspaceRequest(string Name);
public record SendWorkspaceInviteRequest(string Email, WorkspaceRole Role);
public record WorkspaceResponse(Guid Id, string Name, bool IsPersonal);
public record SendWorkspaceInviteResponse(
    Guid Id,
    Guid WorkspaceId,
    string Email,
    WorkspaceRole Role,
    string Token,
    DateTime ExpiresAt);
public record AcceptInviteResponse(Guid WorkspaceId, WorkspaceRole Role);
