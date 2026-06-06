namespace Todo.Application.Notifications;

public record ActivityItemDto(
    Guid Id,
    string ActorName,
    string Message,
    DateTime CreatedAt);

public record NotificationItemDto(
    Guid Id,
    string Message,
    Guid ListId,
    Guid? TaskId,
    bool IsRead,
    DateTime CreatedAt);

public record NotificationSummaryDto(
    int UnreadCount,
    IReadOnlyList<NotificationItemDto> Items);
