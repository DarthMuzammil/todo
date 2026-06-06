using Todo.Application.Activity;
using Todo.Domain.Enums;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Tests.Activity;

[TestFixture]
public class ActivityMessageFormatterTests
{
    [Test]
    public void Format_TaskStatusChangedToDone_UsesDoneMessage()
    {
        var message = ActivityMessageFormatter.Format(
            ActivityAction.TaskStatusChanged,
            "Alex",
            "Buy milk",
            "Groceries",
            TaskStatus.Todo,
            TaskStatus.Done);

        Assert.That(message, Is.EqualTo("Alex marked \"Buy milk\" done"));
    }
}
