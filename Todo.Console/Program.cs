using Todo.Application.Commands.CreateList;
using Todo.Application.Commands.CreateTask;
using Todo.Application.Commands.DeleteTask;
using Todo.Application.Commands.UpdateTaskStatus;
using Todo.Application.Identity;
using Todo.Application.Queries.GetListById;
using Todo.Application.Queries.GetTasksByListId;
using Todo.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application;
using Todo.Infrastructure;
using Todo.Infrastructure.Persistence;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ConnectionStrings:Default"] = "Data Source=data/todo.db"
    })
    .Build();

var services = new ServiceCollection();
services.AddInfrastructure(configuration, "data/tasks.json", "data/lists.json");
services.AddApplication();

var provider = services.BuildServiceProvider();

var consoleUserId = Guid.NewGuid();
await SeedConsoleUserAsync(provider, consoleUserId);

var listHandler = provider.GetRequiredService<CreateListHandler>();
var handler = provider.GetRequiredService<CreateTaskHandler>();
var queryHandler = provider.GetRequiredService<GetTasksByListIdHandler>();
var deleteHandler = provider.GetRequiredService<DeleteTaskHandler>();
var updateHandler = provider.GetRequiredService<UpdateTaskStatusHandler>();
var selectListHandler = provider.GetRequiredService<GetListByIdHandler>();
Guid? currentListId = null;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Todo App ===");
    Console.WriteLine("1. Create list");
    Console.WriteLine("2. Select list");
    Console.WriteLine("3. Create task");
    Console.WriteLine("4. List tasks");
    Console.WriteLine("5. Update status");
    Console.WriteLine("6. Delete task");
    Console.WriteLine("7. Exit");
    Console.Write("Choose: ");

    var input = Console.ReadLine();

    switch (input)
    {
        case "1":
            Console.WriteLine("List Title");
            var title = Console.ReadLine() ?? "Untitled";
            Console.WriteLine("Color: ");
            var color = Console.ReadLine() ?? "#3B82F6";

            var listResult = await listHandler.HandleAsync(new CreateListCommand(
                UserId: consoleUserId,
                Title: title,
                Color: color));

            if (listResult.IsSuccess)
            {
                currentListId = listResult.Value!.Id;
                Console.WriteLine($"Created List: {listResult.Value.Title} ({currentListId})");
            }
            else
            {
                Console.WriteLine($"Failed {listResult.Error}");
            }
            break;
        case "2":
            Console.WriteLine("Enter List Id");
            if (!Guid.TryParse(Console.ReadLine(), out var listId))
            {
                Console.WriteLine("Invalid List Id");
                break;
            }
            var result = await selectListHandler.HandleAsync(new GetListByIdQuery(listId, consoleUserId));
            if (result.IsSuccess)
            {
                currentListId = result.Value!.Id;
                Console.WriteLine($"Selected - {result.Value!.Title}");
            }
            else
            {
                Console.WriteLine($"Error: {result.Error}");
            }
            break;
        case "3":
            if (currentListId is null)
            {
                Console.WriteLine("Please create a list first");
                break;
            }
            Console.WriteLine("Task Title: ");
            var taskTitle = Console.ReadLine() ?? "Untitled";
            var taskResult = await handler.HandleAsync(new CreateTaskCommand(
                ListId: currentListId.Value,
                Title: taskTitle,
                Description: "",
                Priority: Priority.Medium,
                DueDate: null,
                UserId: consoleUserId
            ));
            if (taskResult.IsSuccess)
            {
                Console.WriteLine($"Created Task: {taskResult.Value!.Title} ({taskResult.Value.Id})");
            }
            else
            {
                Console.WriteLine($"Failed {taskResult.Error}");
            }
            break;
        case "4":
            if (currentListId is null)
            {
                Console.WriteLine("Please create a list first");
                break;
            }
            var tasksResult = await queryHandler.HandleAsync(new GetTasksByListIdQuery(currentListId.Value, consoleUserId));
            if (tasksResult.IsSuccess)
            {
                Console.WriteLine($"Tasks in list ({tasksResult.Value!.Count})");
                foreach (var t in tasksResult.Value!)
                {
                    Console.WriteLine($" - {t.Title} [{t.Status}]");
                }
            }
            else
            {
                Console.WriteLine($"Failed: {tasksResult.Error}");
            }
            break;
        case "5":
            Console.WriteLine("Task Id: ");
            if (!Guid.TryParse(Console.ReadLine(), out var taskId))
            {
                Console.WriteLine("Invalid Task Id, please enter correct one: ");
                break;
            }
            Console.WriteLine("Status 0=Todo 1=InProgress 2=Done 3=Cancelled");
            Console.WriteLine("New Status: ");
            if (!int.TryParse(Console.ReadLine(), out var taskStatus) || !Enum.IsDefined(typeof(TaskStatus), taskStatus))
            {
                Console.WriteLine("Invalid Status");
                break;
            }
            var newStatus = (TaskStatus)taskStatus;
            var updateResult = await updateHandler.HandleAsync(
                new UpdateTaskStatusCommand(taskId, newStatus, consoleUserId));
            if (updateResult.IsSuccess)
            {
                Console.WriteLine($"Updated to {updateResult.Value!.Status}");
            }
            else
            {
                Console.WriteLine($"Failed: {updateResult.Error}");
            }
            break;
        case "6":
            Console.WriteLine("Enter the TaskId to delete: ");
            if (!Guid.TryParse(Console.ReadLine(), out var deleteTaskId))
            {
                Console.WriteLine("Invalid Input");
                break;
            }
            var deleteResult = await deleteHandler.HandleAsync(new DeleteTaskCommand(deleteTaskId, consoleUserId));
            if (deleteResult.IsSuccess)
            {
                Console.WriteLine("Deleted Successfully");
            }
            else
            {
                Console.WriteLine($"Failed: {deleteResult.Error}");
            }
            break;
        case "7":
            return;
        default:
            Console.WriteLine("Not implemented yet.");
            break;
    }
}

static async Task SeedConsoleUserAsync(ServiceProvider provider, Guid userId)
{
    using var scope = provider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    await context.Database.MigrateAsync();

    if (await context.Users.AnyAsync(u => u.Id == userId))
        return;

    var email = $"console-{userId:N}@local.test";
    context.Users.Add(new ApplicationUser
    {
        Id = userId,
        Name = "Console User",
        Email = email,
        UserName = email,
        NormalizedEmail = email.ToUpperInvariant(),
        NormalizedUserName = email.ToUpperInvariant()
    });
    await context.SaveChangesAsync();
}