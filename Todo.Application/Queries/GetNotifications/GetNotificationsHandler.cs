using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Application.Notifications;

namespace Todo.Application.Queries.GetNotifications;

public class GetNotificationsHandler
{
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationsHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<NotificationSummaryDto>> HandleAsync(GetNotificationsQuery query)
    {
        var notificationsResult = await _notificationRepository.GetForUserAsync(
            query.UserId,
            query.Limit);

        if (!notificationsResult.IsSuccess)
            return Result<NotificationSummaryDto>.Failure(notificationsResult.Error!);

        var unreadCount = await _notificationRepository.GetUnreadCountAsync(query.UserId);

        var items = notificationsResult.Value!
            .Select(notification => new NotificationItemDto(
                notification.Id,
                notification.Message,
                notification.ListId,
                notification.TaskId,
                notification.IsRead,
                notification.CreatedAt))
            .ToList();

        return Result<NotificationSummaryDto>.Success(
            new NotificationSummaryDto(unreadCount, items));
    }
}
