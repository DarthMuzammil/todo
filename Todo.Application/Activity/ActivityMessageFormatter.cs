using Todo.Domain.Enums;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Application.Activity;

public static class ActivityMessageFormatter
{
    public static string Format(
        ActivityAction action,
        string actorName,
        string taskTitle,
        string listTitle,
        TaskStatus? previousStatus = null,
        TaskStatus? newStatus = null)
    {
        return action switch
        {
            ActivityAction.TaskCreated =>
                $"{actorName} added \"{taskTitle}\"",
            ActivityAction.TaskStatusChanged when newStatus == TaskStatus.Done =>
                $"{actorName} marked \"{taskTitle}\" done",
            ActivityAction.TaskStatusChanged when previousStatus == TaskStatus.Done =>
                $"{actorName} reopened \"{taskTitle}\"",
            ActivityAction.TaskStatusChanged =>
                $"{actorName} updated \"{taskTitle}\"",
            ActivityAction.TaskDeleted =>
                $"{actorName} deleted \"{taskTitle}\"",
            ActivityAction.ListCreated =>
                $"{actorName} created list \"{listTitle}\"",
            ActivityAction.ListUpdated =>
                $"{actorName} renamed the list to \"{listTitle}\"",
            ActivityAction.ListDeleted =>
                $"{actorName} deleted list \"{listTitle}\"",
            _ => $"{actorName} updated the list",
        };
    }
}
