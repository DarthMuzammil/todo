namespace Todo.Application.Queries.GetTasksByListId;
public record GetTasksByListIdQuery(Guid ListId, Guid UserId);