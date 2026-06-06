using Todo.Application.Common;

namespace Todo.Tests.Common;

[TestFixture]
public class ResultTests
{
    [Test]
    public void Result_Success_IsSuccessful()
    {
        // Arrange & Act
        var result = Result.Success();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public void Result_Failure_HasError()
    {
        // Arrange & Act
        var result = Result.Failure("Error message");

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Error message"));
    }

    [Test]
    public void Result_Success_HasValue()
    {
        // Arrange & Act
        var result = Result<string>.Success("Success message");

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Success message"));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public void Result_Failure_HasNoValue()
    {
        // Arrange & Act
        var result = Result<string>.Failure("Error message");

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.Error, Is.EqualTo("Error message"));
    }
}