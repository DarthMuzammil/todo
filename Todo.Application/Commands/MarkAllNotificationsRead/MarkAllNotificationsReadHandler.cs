using Todo.Application.Common;
using Todo.Application.Interfaces;

namespace Todo.Application.Commands.MarkAllNotificationsRead;

public class MarkAllNotificationsReadHandler
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllNotificationsReadHandler(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(MarkAllNotificationsReadCommand command)
    {
        await _notificationRepository.MarkAllReadAsync(command.UserId);
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
