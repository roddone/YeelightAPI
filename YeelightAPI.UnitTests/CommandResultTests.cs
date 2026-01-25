using YeelightAPI.Models;

namespace YeelightAPI.UnitTests;

public class CommandResultTests
{
    #region CommandResult Basic Tests

    [Test]
    public async Task CommandResult_DefaultValues()
    {
        var result = new CommandResult();

        await Assert.That(result.Id).IsEqualTo(0);
        await Assert.That(result.Error).IsNull();
    }

    [Test]
    public async Task CommandResult_SetProperties()
    {
        var result = new CommandResult
        {
            Id = 42,
            Error = new CommandResult.CommandErrorResult
            {
                Code = -1,
                Message = "Test error"
            }
        };

        await Assert.That(result.Id).IsEqualTo(42);
        await Assert.That(result.Error).IsNotNull();
        await Assert.That(result.Error!.Code).IsEqualTo(-1);
        await Assert.That(result.Error.Message).IsEqualTo("Test error");
    }

    #endregion

    #region CommandResult<T> Tests

    [Test]
    public async Task CommandResultGeneric_WithStringResult()
    {
        var result = new CommandResult<string>
        {
            Id = 1,
            Result = "ok"
        };

        await Assert.That(result.Id).IsEqualTo(1);
        await Assert.That(result.Result).IsEqualTo("ok");
        await Assert.That(result.Error).IsNull();
    }

    [Test]
    public async Task CommandResultGeneric_WithListResult()
    {
        var result = new CommandResult<List<string>>
        {
            Id = 2,
            Result = new List<string> { "ok" }
        };

        await Assert.That(result.Id).IsEqualTo(2);
        await Assert.That(result.Result).HasSingleItem();
        await Assert.That(result.Result![0]).IsEqualTo("ok");
    }

    [Test]
    public async Task CommandResultGeneric_WithIntResult()
    {
        var result = new CommandResult<int>
        {
            Id = 3,
            Result = 100
        };

        await Assert.That(result.Result).IsEqualTo(100);
    }

    [Test]
    public async Task CommandResultGeneric_InheritsFromCommandResult()
    {
        var result = new CommandResult<string>();

        await Assert.That(result).IsAssignableTo<CommandResult>();
    }

    #endregion

    #region IsOk Extension Tests

    [Test]
    public async Task IsOk_WithOkResult_ReturnsTrue()
    {
        var result = new CommandResult<List<string>>
        {
            Id = 1,
            Result = new List<string> { "ok" }
        };

        await Assert.That(result.IsOk()).IsTrue();
    }

    [Test]
    public async Task IsOk_WithError_ReturnsFalse()
    {
        var result = new CommandResult<List<string>>
        {
            Id = 1,
            Error = new CommandResult.CommandErrorResult
            {
                Code = -1,
                Message = "Error"
            },
            Result = new List<string> { "ok" }
        };

        await Assert.That(result.IsOk()).IsFalse();
    }

    [Test]
    public async Task IsOk_WithNonOkResult_ReturnsFalse()
    {
        var result = new CommandResult<List<string>>
        {
            Id = 1,
            Result = new List<string> { "error" }
        };

        await Assert.That(result.IsOk()).IsFalse();
    }

    [Test]
    public async Task IsOk_WithEmptyResult_ThrowsException()
    {
        var result = new CommandResult<List<string>>
        {
            Id = 1,
            Result = new List<string>()
        };

        await Assert.That(() => result.IsOk()).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task IsOk_WithNullResult_ReturnsFalse()
    {
        var result = new CommandResult<List<string>>
        {
            Id = 1,
            Result = null
        };

        await Assert.That(result.IsOk()).IsFalse();
    }

    [Test]
    public async Task IsOk_WithNullCommandResult_ReturnsFalse()
    {
        CommandResult<List<string>>? result = null;

        await Assert.That(result.IsOk()).IsFalse();
    }

    #endregion

    #region CommandErrorResult Tests

    [Test]
    public async Task CommandErrorResult_ToString_FormatsCorrectly()
    {
        var error = new CommandResult.CommandErrorResult
        {
            Code = -5000,
            Message = "general error"
        };

        await Assert.That(error.ToString()).IsEqualTo("-5000 - general error");
    }

    [Test]
    public async Task CommandErrorResult_ToString_WithEmptyMessage()
    {
        var error = new CommandResult.CommandErrorResult
        {
            Code = -1,
            Message = ""
        };

        await Assert.That(error.ToString()).IsEqualTo("-1 - ");
    }

    [Test]
    public async Task CommandErrorResult_ToString_WithNullMessage()
    {
        var error = new CommandResult.CommandErrorResult
        {
            Code = -1,
            Message = null!
        };

        await Assert.That(error.ToString()).IsEqualTo("-1 - ");
    }

    [Test]
    [Arguments(-1, "Invalid command")]
    [Arguments(-2, "Invalid params")]
    [Arguments(-3, "Method not supported")]
    [Arguments(-4, "Quota exceeded")]
    [Arguments(-5000, "General error")]
    public async Task CommandErrorResult_CommonErrorCodes(int code, string message)
    {
        var error = new CommandResult.CommandErrorResult
        {
            Code = code,
            Message = message
        };

        await Assert.That(error.Code).IsEqualTo(code);
        await Assert.That(error.Message).IsEqualTo(message);
    }

    #endregion
}
