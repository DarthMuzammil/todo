namespace Todo.Application.Commands.Register;

public record RegisterCommand(string Email, string Password, string Name);