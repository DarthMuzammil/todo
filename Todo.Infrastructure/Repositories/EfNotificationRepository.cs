using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.Interfaces;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Repositories;

public class EfNotificationRepository : INotificationRepository
{
    private readonly TodoDbContext _context;

    public EfNotificationRepository(TodoDbContext context)
    {
        _context = context;
    }

    public Task AddRangeAsync(IEnumerable<UserNotification> notifications)
    {
        _context.UserNotifications.AddRange(notifications);
        return Task.CompletedTask;
    }

    public async Task<Result<List<UserNotification>>> GetForUserAsync(Guid userId, int limit = 20)
    {
        var notifications = await _context.UserNotifications
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return Result<List<UserNotification>>.Success(notifications);
    }

    public Task<int> GetUnreadCountAsync(Guid userId) =>
        _context.UserNotifications.CountAsync(
            notification => notification.UserId == userId && !notification.IsRead);

    public async Task<Result<UserNotification>> MarkReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _context.UserNotifications
            .FirstOrDefaultAsync(item =>
                item.Id == notificationId && item.UserId == userId);

        if (notification is null)
            return Result<UserNotification>.Failure("Notification not found.");

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        return Result<UserNotification>.Success(notification);
    }

    public async Task MarkAllReadAsync(Guid userId)
    {
        var unread = await _context.UserNotifications
            .Where(notification => notification.UserId == userId && !notification.IsRead)
            .ToListAsync();

        if (unread.Count == 0)
            return;

        var now = DateTime.UtcNow;
        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
        }
    }
}
