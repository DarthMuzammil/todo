using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Interfaces;
using Todo.Infrastructure.Repositories;

namespace Todo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string tasksFilePath,
        string listsFilePath)
    {
        services.AddSingleton<ITaskRepository>(_ => new JsonTaskRepository(tasksFilePath));
        services.AddSingleton<IListRepository>(_ => new JsonListRepository(listsFilePath));
        return services;
    }
}