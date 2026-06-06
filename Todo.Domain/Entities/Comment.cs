namespace Todo.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid AuthorId { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
}
