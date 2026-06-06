namespace Todo.Application.Queries.GetNotifications;

public record GetNotificationsQuery(Guid UserId, int Limit = 20);
