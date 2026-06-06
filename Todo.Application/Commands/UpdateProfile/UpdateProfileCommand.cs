namespace Todo.Application.Commands.UpdateProfile;

public record UpdateProfileCommand(Guid UserId, string Name);
