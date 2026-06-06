using Todo.Domain.Enums;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Domain.Entities;

public class TodoTask
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid? ParentTaskId { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public long Version { get; set; } = 1;
    public Guid ListId { get; set; }

}
