using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Common;
using Todo.Application.Commands.Login;
using Todo.Application.Commands.Logout;
using Todo.Application.Commands.RefreshToken;
using Todo.Application.Commands.Register;
using Todo.Application.Commands.AcceptWorkspaceInvite;
using Todo.Application.Commands.CreateList;
using Todo.Application.Commands.CreateSharedWorkspace;
using Todo.Application.Commands.DeclineWorkspaceInvite;
using Todo.Application.Commands.RemoveWorkspaceMember;
using Todo.Application.Commands.UpdateProfile;
using Todo.Application.Commands.ChangePassword;
using Todo.Application.Commands.LogoutAllSessions;
using Todo.Application.Commands.ResendWorkspaceInvite;
using Todo.Application.Commands.SendWorkspaceInvite;
using Todo.Application.Commands.CreateTask;
using Todo.Application.Commands.DeleteList;
using Todo.Application.Commands.DeleteTask;
using Todo.Application.Commands.UpdateList;
using Todo.Application.Commands.UpdateTaskStatus;
using Todo.Application.Queries.GetListById;
using Todo.Application.Queries.GetListActivity;
using Todo.Application.Queries.GetListsByOwnerId;
using Todo.Application.Queries.GetWorkspaceMembers;
using Todo.Application.Queries.GetWorkspaceInvites;
using Todo.Application.Queries.GetWorkspaces;
using Todo.Application.Queries.GetTasksByListId;
using Todo.Application.Activity;
using Todo.Application.Commands.MarkAllNotificationsRead;
using Todo.Application.Commands.MarkNotificationRead;
using Todo.Application.Queries.GetNotifications;

using Todo.Application.Realtime;

namespace Todo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ListAccessChecker>();
        services.AddScoped<WorkspaceMembershipChecker>();
        services.AddScoped<CreateSharedWorkspaceHandler>();
        services.AddScoped<SendWorkspaceInviteHandler>();
        services.AddScoped<AcceptWorkspaceInviteHandler>();
        services.AddScoped<DeclineWorkspaceInviteHandler>();
        services.AddScoped<GetWorkspacesHandler>();
        services.AddScoped<GetWorkspaceInvitesHandler>();
        services.AddScoped<GetWorkspaceMembersHandler>();
        services.AddScoped<RemoveWorkspaceMemberHandler>();
        services.AddScoped<ResendWorkspaceInviteHandler>();
        services.AddScoped<UpdateProfileHandler>();
        services.AddScoped<ChangePasswordHandler>();
        services.AddScoped<LogoutAllSessionsHandler>();
        services.AddSingleton<IListSyncNotifier, NullListSyncNotifier>();
        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<RefreshTokenHandler>();
        services.AddScoped<LogoutHandler>();
        services.AddScoped<CreateListHandler>();
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<GetTasksByListIdHandler>();
        services.AddScoped<UpdateListHandler>();
        services.AddScoped<UpdateTaskStatusHandler>();
        services.AddScoped<DeleteTaskHandler>();
        services.AddScoped<DeleteListHandler>();
        services.AddScoped<GetListByIdHandler>();
        services.AddScoped<GetListsByOwnerIdHandler>();
        services.AddScoped<PersonalWorkspaceService>();
        services.AddScoped<ActivityRecorder>();
        services.AddScoped<GetListActivityHandler>();
        services.AddScoped<GetNotificationsHandler>();
        services.AddScoped<MarkNotificationReadHandler>();
        services.AddScoped<MarkAllNotificationsReadHandler>();

        return services;
    }
}
