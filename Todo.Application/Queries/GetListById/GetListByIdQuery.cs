namespace Todo.Application.Queries.GetListById;

public record GetListByIdQuery(Guid ListId, Guid UserId);