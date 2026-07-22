namespace WebApp.Api.Tests.Services;

public class JsonFormatterServiceTests
{
    private readonly JsonFormatterService _service = new();


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Execute_ShouldThrow_WhenInputIsEmpty(string? input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input!));
    }


    [Theory]
    [InlineData("pretty")]
    [InlineData("minify")]
    [InlineData("pretty\n")]
    [InlineData("minify\n")]
    public void Execute_ShouldThrow_WhenJsonIsMissing(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Fact]
    public void Execute_ShouldFormatJson_WhenModeIsPretty()
    {
        var input =
            """
            pretty
            {"a":1,"b":[2,3]}
            """;


        var result = _service.Execute(input);


        Assert.Contains("\"a\": 1", result);
        Assert.Contains("\"b\": [", result);
        Assert.Contains("2", result);
        Assert.Contains("3", result);
    }


    [Fact]
    public void Execute_ShouldMinifyJson_WhenModeIsMinify()
    {
        var input =
            """
            minify
            {
                "name": "Ivan",
                "age": 20
            }
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            """{"name":"Ivan","age":20}""",
            result
        );
    }


    [Fact]
    public void Execute_ShouldKeepNestedObjects_WhenFormattingPretty()
    {
        var input =
            """
            pretty
            {"user":{"name":"Ivan","age":20}}
            """;


        var result = _service.Execute(input);


        Assert.Contains("\"user\": {", result);
        Assert.Contains("\"name\": " + "\"Ivan\"", result);
        Assert.Contains("\"age\": 20", result);
    }


    [Fact]
    public void Execute_ShouldFormatArrays_WhenJsonContainsArray()
    {
        var input =
            """
            pretty
            {"items":[1,2,3]}
            """;


        var result = _service.Execute(input);


        Assert.Contains("\"items\": [", result);
        Assert.Contains("1", result);
        Assert.Contains("2", result);
        Assert.Contains("3", result);
    }


    [Fact]
    public void Execute_ShouldReturnSameJson_WhenMinifyAlreadyMinified()
    {
        var input =
            """
            minify
            {"a":1,"b":2}
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            """{"a":1,"b":2}""",
            result
        );
    }


    [Theory]
    [InlineData("{")]
    [InlineData("abc")]
    [InlineData("[1,2,")]
    public void Execute_ShouldThrow_WhenJsonIsInvalid(string json)
    {
        var input =
            $"pretty\n{json}";


        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Fact]
    public void Execute_ShouldThrow_WhenModeIsInvalid()
    {
        var input =
            """
            format
            {"a":1}
            """;


        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Fact]
    public void Execute_ShouldWorkWithDifferentModeCases()
    {
        var input =
            """
            PRETTY
            {"a":1}
            """;


        var result = _service.Execute(input);


        Assert.Contains("\"a\": 1", result);
    }


    [Fact]
    public void Execute_ShouldHandleEmptyJsonObject()
    {
        var input =
            """
            pretty
            {}
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "{}",
            result
        );
    }


    [Fact]
    public void Execute_ShouldHandleJsonArray()
    {
        var input =
            """
            pretty
            [1,2,3]
            """;


        var result = _service.Execute(input);


        Assert.Contains("[", result);
        Assert.Contains("1", result);
        Assert.Contains("2", result);
        Assert.Contains("3", result);
    }
}