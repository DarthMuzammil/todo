using Todo.Domain.Enums;

namespace Todo.Domain.Entities;

public class ActivityEntry
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid ListId { get; set; }
    public Guid? TaskId { get; set; }
    public Guid ActorUserId { get; set; }
    public ActivityAction Action { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public string ListTitle { get; set; } = string.Empty;
    public int? PreviousStatus { get; set; }
    public int? NewStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}
