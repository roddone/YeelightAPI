using System.Text.Json;
using YeelightAPI.Models;

namespace YeelightAPI.UnitTests;

public class SerializationTests
{
    private static readonly JsonSerializerOptions CamelCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    #region Command Serialization Tests

    [Test]
    public async Task Command_Serialization_ProducesCamelCase()
    {
        var command = new Command
        {
            Id = 1,
            Method = "set_power",
            Params = new List<object> { "on", "smooth", 500 }
        };

        var json = JsonSerializer.Serialize(command, CamelCaseOptions);

        await Assert.That(json).Contains("\"id\":1");
        await Assert.That(json).Contains("\"method\":\"set_power\"");
        await Assert.That(json).Contains("\"params\":");
    }

    [Test]
    public async Task Command_Serialization_WithEmptyParams()
    {
        var command = new Command
        {
            Id = 2,
            Method = "toggle",
            Params = new List<object>()
        };

        var json = JsonSerializer.Serialize(command, CamelCaseOptions);

        await Assert.That(json).Contains("\"id\":2");
        await Assert.That(json).Contains("\"method\":\"toggle\"");
        await Assert.That(json).Contains("\"params\":[]");
    }

    [Test]
    public async Task Command_Serialization_WithMixedParams()
    {
        var command = new Command
        {
            Id = 3,
            Method = "set_rgb",
            Params = new List<object> { 16711680, "smooth", 500 }
        };

        var json = JsonSerializer.Serialize(command, CamelCaseOptions);

        await Assert.That(json).Contains("16711680");
        await Assert.That(json).Contains("\"smooth\"");
        await Assert.That(json).Contains("500");
    }

    [Test]
    public async Task Command_Deserialization()
    {
        var json = """{"id":5,"method":"get_prop","params":["power","bright"]}""";

        var command = JsonSerializer.Deserialize<Command>(json, CamelCaseOptions);

        await Assert.That(command).IsNotNull();
        await Assert.That(command!.Id).IsEqualTo(5);
        await Assert.That(command.Method).IsEqualTo("get_prop");
        await Assert.That(command.Params.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Command_RoundTrip()
    {
        var original = new Command
        {
            Id = 10,
            Method = "set_bright",
            Params = new List<object> { 50 }
        };

        var json = JsonSerializer.Serialize(original, CamelCaseOptions);
        var deserialized = JsonSerializer.Deserialize<Command>(json, CamelCaseOptions);

        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Id).IsEqualTo(original.Id);
        await Assert.That(deserialized.Method).IsEqualTo(original.Method);
    }

    #endregion

    #region CommandResult Serialization Tests

    [Test]
    public async Task CommandResult_Deserialization_Success()
    {
        var json = """{"id":1,"result":["ok"]}""";

        var result = JsonSerializer.Deserialize<CommandResult<List<string>>>(json, CamelCaseOptions);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsEqualTo(1);
        await Assert.That(result.Error).IsNull();
        await Assert.That(result.Result).HasSingleItem();
        await Assert.That(result.Result![0]).IsEqualTo("ok");
    }

    [Test]
    public async Task CommandResult_Deserialization_WithError()
    {
        var json = """{"id":2,"error":{"code":-1,"message":"invalid command"}}""";

        var result = JsonSerializer.Deserialize<CommandResult<List<string>>>(json, CamelCaseOptions);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsEqualTo(2);
        await Assert.That(result.Error).IsNotNull();
        await Assert.That(result.Error!.Code).IsEqualTo(-1);
        await Assert.That(result.Error.Message).IsEqualTo("invalid command");
    }

    [Test]
    public async Task CommandResult_Deserialization_GetProp()
    {
        var json = """{"id":3,"result":["on","100","4000"]}""";

        var result = JsonSerializer.Deserialize<CommandResult<List<string>>>(json, CamelCaseOptions);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsEqualTo(3);
        await Assert.That(result.Result!.Count).IsEqualTo(3);
        await Assert.That(result.Result[0]).IsEqualTo("on");
        await Assert.That(result.Result[1]).IsEqualTo("100");
        await Assert.That(result.Result[2]).IsEqualTo("4000");
    }

    [Test]
    public async Task CommandResult_Serialization()
    {
        var result = new CommandResult<List<string>>
        {
            Id = 1,
            Result = new List<string> { "ok" }
        };

        var json = JsonSerializer.Serialize(result, CamelCaseOptions);

        await Assert.That(json).Contains("\"id\":1");
        await Assert.That(json).Contains("\"result\":[\"ok\"]");
    }

    #endregion

    #region NotificationResult Serialization Tests

    [Test]
    public async Task NotificationResult_Deserialization()
    {
        var json = """{"method":"props","params":{"power":"on","bright":"50"}}""";

        var result = JsonSerializer.Deserialize<NotificationResult>(json, CamelCaseOptions);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Method).IsEqualTo("props");
    }

    #endregion

    #region Edge Cases

    [Test]
    public async Task Command_WithNullParams()
    {
        var command = new Command
        {
            Id = 1,
            Method = "test",
            Params = null!
        };

        var json = JsonSerializer.Serialize(command, CamelCaseOptions);

        await Assert.That(json).Contains("\"params\":null");
    }

    [Test]
    public async Task Command_WithLargeId()
    {
        var command = new Command
        {
            Id = int.MaxValue,
            Method = "test",
            Params = new List<object>()
        };

        var json = JsonSerializer.Serialize(command, CamelCaseOptions);
        var deserialized = JsonSerializer.Deserialize<Command>(json, CamelCaseOptions);

        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Id).IsEqualTo(int.MaxValue);
    }

    [Test]
    public async Task CommandResult_WithNestedParams()
    {
        var json = """{"id":1,"result":[["value1","value2"],["value3"]]}""";

        var result = JsonSerializer.Deserialize<CommandResult<List<List<string>>>>(json, CamelCaseOptions);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Result!.Count).IsEqualTo(2);
        await Assert.That(result.Result[0].Count).IsEqualTo(2);
        await Assert.That(result.Result[1]).HasSingleItem();
    }

    #endregion

    #region Protocol-Specific Tests

    [Test]
    public async Task Command_SetPower_CorrectFormat()
    {
        var command = new Command
        {
            Id = 1,
            Method = "set_power",
            Params = new List<object> { "on", "smooth", 500 }
        };

        var json = JsonSerializer.Serialize(command, CamelCaseOptions);

        // Should match Yeelight protocol format
        await Assert.That(json).DoesNotContain("Params"); // Should be lowercase
        await Assert.That(json).DoesNotContain("Method"); // Should be lowercase
        await Assert.That(json).DoesNotContain("Id");     // Should be lowercase
    }

    [Test]
    public async Task Command_SetRGB_WithColorValue()
    {
        int redColor = (255 << 16) | (0 << 8) | 0; // Red
        var command = new Command
        {
            Id = 1,
            Method = "set_rgb",
            Params = new List<object> { redColor, "smooth", 500 }
        };

        var json = JsonSerializer.Serialize(command, CamelCaseOptions);

        await Assert.That(json).Contains("16711680"); // Red color value
    }

    [Test]
    public async Task Command_StartColorFlow_WithExpression()
    {
        var command = new Command
        {
            Id = 1,
            Method = "start_cf",
            Params = new List<object> { 0, 0, "500,1,16711680,100,1000,7,0,0" }
        };

        var json = JsonSerializer.Serialize(command, CamelCaseOptions);

        await Assert.That(json).Contains("\"start_cf\"");
        await Assert.That(json).Contains("500,1,16711680,100,1000,7,0,0");
    }

    #endregion
}
