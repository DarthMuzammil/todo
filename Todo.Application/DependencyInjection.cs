using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Commands.CreateList;
using Todo.Application.Commands.CreateTask;
using Todo.Application.Commands.DeleteTask;
using Todo.Application.Commands.UpdateTaskStatus;
using Todo.Application.Queries.GetListById;
using Todo.Application.Queries.GetListsByOwnerId;
using Todo.Application.Queries.GetTasksByListId;

namespace Todo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateListHandler>();
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<GetTasksByListIdHandler>();
        services.AddScoped<UpdateTaskStatusHandler>();
        services.AddScoped<DeleteTaskHandler>();
        services.AddScoped<GetListByIdHandler>();
        services.AddScoped<GetListsByOwnerIdHandler>();

        return services;
    }
}
