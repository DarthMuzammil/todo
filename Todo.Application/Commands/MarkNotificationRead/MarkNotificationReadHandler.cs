using Todo.Application.Common;
using Todo.Application.Interfaces;

namespace Todo.Application.Commands.MarkNotificationRead;

public class MarkNotificationReadHandler
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationReadHandler(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(MarkNotificationReadCommand command)
    {
        var result = await _notificationRepository.MarkReadAsync(
            command.NotificationId,
            command.UserId);

        if (!result.IsSuccess)
            return Result.Failure(result.Error!);

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
