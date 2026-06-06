namespace Todo.Infrastructure.Persistence;

public sealed record JsonImportResult(
    int UsersImported,
    int ListsImported,
    int TasksImported,
    int TasksSkipped);
