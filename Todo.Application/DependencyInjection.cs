using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Commands.CreateList;
using Todo.Application.Commands.CreateTask;
using Todo.Application.Commands.DeleteTask;
using Todo.Application.Commands.UpdateTaskStatus;
using Todo.Application.Queries.GetListById;
using Todo.Application.Queries.GetTasksByListId;

namespace Todo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<CreateListHandler>();
        services.AddTransient<CreateTaskHandler>();
        services.AddTransient<GetTasksByListIdHandler>();
        services.AddTransient<UpdateTaskStatusHandler>();
        services.AddTransient<DeleteTaskHandler>();
        services.AddTransient<GetListByIdHandler>();

        return services;
    }
}
