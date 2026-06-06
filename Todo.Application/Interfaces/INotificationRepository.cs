using Todo.Application.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Interfaces;

public interface INotificationRepository
{
    Task AddRangeAsync(IEnumerable<UserNotification> notifications);
    Task<Result<List<UserNotification>>> GetForUserAsync(Guid userId, int limit = 20);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<Result<UserNotification>> MarkReadAsync(Guid notificationId, Guid userId);
    Task MarkAllReadAsync(Guid userId);
}
