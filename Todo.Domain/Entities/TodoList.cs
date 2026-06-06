namespace Todo.Domain.Entities;

public class TodoList
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; }
    public string Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
