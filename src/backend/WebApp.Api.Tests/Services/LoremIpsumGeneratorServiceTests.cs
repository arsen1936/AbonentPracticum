namespace WebApp.Api.Tests.Services;

public class LoremIpsumGeneratorServiceTests
{
    private readonly LoremIpsumGeneratorService _service = new();

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
    public void Execute_ShouldGenerateParagraphs()
    {
        var input =
            """
            {
                "type":"paragraphs",
                "count":"2"
            }
            """;

        var result = _service.Execute(input);

        Assert.Contains("Lorem ipsum", result);
        Assert.Equal(2, result.Split(Environment.NewLine + Environment.NewLine).Length);
    }

    [Fact]
    public void Execute_ShouldGenerateWords()
    {
        var input =
            """
            {
                "type":"words",
                "count":"10"
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal(10, result.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
    }

    [Fact]
    public void Execute_ShouldGenerateChars()
    {
        var input =
            """
            {
                "type":"chars",
                "count":"50"
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal(50, result.Length);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("abc")]
    [InlineData("[1,2,3]")]
    public void Execute_ShouldThrow_WhenJsonIsInvalid(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenTypeIsMissing()
    {
        var input =
            """
            {
                "count":"10"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenCountIsMissing()
    {
        var input =
            """
            {
                "type":"words"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("10.5")]
    public void Execute_ShouldThrow_WhenCountIsInvalid(string count)
    {
        var input =
            $$"""
            {
                "type":"words",
                "count":"{{count}}"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-100")]
    public void Execute_ShouldThrow_WhenCountIsNotPositive(string count)
    {
        var input =
            $$"""
            {
                "type":"words",
                "count":"{{count}}"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenTypeIsInvalid()
    {
        var input =
            """
            {
                "type":"lines",
                "count":"10"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }
}