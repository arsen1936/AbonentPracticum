namespace WebApp.Api.Tests.Services;

public class RegularExpressionBuilderServiceTests
{
    private readonly RegularExpressionBuilderService _service = new();

    [Fact]
    public void Execute_ShouldFindMatches()
    {
        var input = """
        {
          "pattern":"\\d+",
          "flags":"",
          "text":"abc123 def456"
        }
        """;
        var result = _service.Execute(input);
        Assert.Contains("Найдено 2 совпадения:", result);
        Assert.Contains("1. \"123\"", result);
        Assert.Contains("2. \"456\"", result);
    }

    [Fact]
    public void Execute_ShouldReturnMessage_WhenNoMatchesFound()
    {
        var input = """
        {
          "pattern":"\\d+",
          "flags":"",
          "text":"abcdef"
        }
        """;
        var result = _service.Execute(input);
        Assert.Equal("Совпадений не найдено.", result);
    }

    [Fact]
    public void Execute_ShouldSupportIgnoreCaseFlag()
    {
        var input = """
        {
          "pattern":"hello",
          "flags":"i",
          "text":"HELLO"
        }
        """;
        var result = _service.Execute(input);
        Assert.Contains("Найдено 1 совпадения:", result);
        Assert.Contains("\"HELLO\"", result);
    }

    [Fact]
    public void Execute_ShouldSupportCapturingGroups()
    {
        var input = """
        {
          "pattern":"(\\w+)=(\\d+)",
          "flags":"",
          "text":"id=123"
        }
        """;
        var result = _service.Execute(input);
        Assert.Contains("1. \"id=123\"", result);
        Assert.Contains("1: \"id\"", result);
        Assert.Contains("2: \"123\"", result);
    }

    [Fact]
    public void Execute_ShouldReturnRegexError_WhenPatternIsInvalid()
    {
        var input = """
        {
          "pattern":"(",
          "flags":"",
          "text":"abc"
        }
        """;
        var result = _service.Execute(input);
        Assert.StartsWith("Ошибка регулярного выражения:", result);
    }

    [Fact]
    public void Execute_ShouldReturnRegexError_WhenFlagIsInvalid()
    {
        var input = """
        {
          "pattern":"abc",
          "flags":"z",
          "text":"abc"
        }
        """;
        var result = _service.Execute(input);
        Assert.StartsWith("Ошибка регулярного выражения:", result);
        Assert.Contains("Неизвестный флаг", result);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenJsonIsInvalid()
    {
        var input = "{ invalid json }";
        var ex = Assert.Throws<ArgumentException>(() => _service.Execute(input));
        Assert.Equal("Некорректный JSON.", ex.Message);
    }

    [Fact]
    public void Execute_ShouldSupportMultilineFlag()
    {
        var input = """
        {
          "pattern":"^abc",
          "flags":"m",
          "text":"123\nabc"
        }
        """;
        var result = _service.Execute(input);
        Assert.Contains("\"abc\"", result);
    }
}