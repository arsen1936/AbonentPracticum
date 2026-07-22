namespace WebApp.Api.Tests.Services;

public class RegisterConverterServiceTests
{
    private readonly RegisterConverterService _service = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Execute_ShouldThrow_WhenInputIsEmpty(string? input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input!));
    }

    [Fact]
    public void Execute_ShouldConvertToUpperCase()
    {
        var input = "UPPER CASE\nHello World";

        var result = _service.Execute(input);

        Assert.Equal("HELLO WORLD", result);
    }

    [Fact]
    public void Execute_ShouldConvertToLowerCase()
    {
        var input = "lower case\nHello World";

        var result = _service.Execute(input);

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void Execute_ShouldConvertToTitleCase()
    {
        var input = "Title Case\nhello world example";

        var result = _service.Execute(input);

        Assert.Equal("Hello World Example", result);
    }

    [Fact]
    public void Execute_ShouldConvertToCamelCase()
    {
        var input = "camelCase\nHello World Example";

        var result = _service.Execute(input);

        Assert.Equal("helloWorldExample", result);
    }

    [Fact]
    public void Execute_ShouldConvertToPascalCase()
    {
        var input = "PascalCase\nhello world example";

        var result = _service.Execute(input);

        Assert.Equal("HelloWorldExample", result);
    }

    [Fact]
    public void Execute_ShouldConvertToSnakeCase()
    {
        var input = "snake_case\nHello World Example";

        var result = _service.Execute(input);

        Assert.Equal("hello_world_example", result);
    }

    [Fact]
    public void Execute_ShouldConvertToKebabCase()
    {
        var input = "kebab-case\nHello World Example";

        var result = _service.Execute(input);

        Assert.Equal("hello-world-example", result);
    }

    [Theory]
    [InlineData("snake_case\nhelloWorldExample", "hello_world_example")]
    [InlineData("snake_case\nHelloWorldExample", "hello_world_example")]
    [InlineData("snake_case\nhello-world-example", "hello_world_example")]
    [InlineData("snake_case\nhello world example", "hello_world_example")]
    public void Execute_ShouldConvertDifferentFormatsToSnakeCase(string input, string expected)
    {
        var result = _service.Execute(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("camelCase")]
    [InlineData("snake_case\n")]
    [InlineData("camelCase\nHello\nWorld")]
    public void Execute_ShouldThrow_WhenInputFormatIsInvalid(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenModeIsInvalid()
    {
        var input = "Train-Case\nHello World";

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }
}