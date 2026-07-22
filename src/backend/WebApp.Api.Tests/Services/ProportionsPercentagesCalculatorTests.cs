namespace WebApp.Api.Tests.Services;

using WebApp.Api.Services;

public class ProportionsPercentagesCalculatorTests
{
    private readonly ProportionsPercentagesCalculator _service = new();

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
    public void Execute_ShouldCalculatePercentOf()
    {
        var input =
            """
            {
                "operation":"percent-of",
                "value1":"30",
                "value2":"200"
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal("30 составляет 15% от 200", result);
    }

    [Fact]
    public void Execute_ShouldCalculateChangeIncrease()
    {
        var input =
            """
            {
                "operation":"change",
                "value1":"200",
                "value2":"250"
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal("Изменение: +25,00% (увеличение на 50)", result);
    }

    [Fact]
    public void Execute_ShouldCalculateChangeDecrease()
    {
        var input =
            """
            {
                "operation":"change",
                "value1":"200",
                "value2":"150"
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal("Изменение: -25,00% (уменьшение на 50)", result);
    }

    [Fact]
    public void Execute_ShouldCalculateProportion()
    {
        var input =
            """
            {
                "operation":"proportion",
                "value1":"100",
                "value2":"20"
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal("100, увеличенное на 20% = 120", result);
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
    public void Execute_ShouldThrow_WhenOperationIsMissing()
    {
        var input =
            """
            {
                "value1":"10",
                "value2":"20"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Theory]
    [InlineData("abc", "20")]
    [InlineData("", "20")]
    public void Execute_ShouldThrow_WhenValue1IsInvalid(string value1, string value2)
    {
        var input =
            $$"""
            {
                "operation":"percent-of",
                "value1":"{{value1}}",
                "value2":"{{value2}}"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Theory]
    [InlineData("10", "abc")]
    [InlineData("10", "")]
    public void Execute_ShouldThrow_WhenValue2IsInvalid(string value1, string value2)
    {
        var input =
            $$"""
            {
                "operation":"percent-of",
                "value1":"{{value1}}",
                "value2":"{{value2}}"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenOperationIsInvalid()
    {
        var input =
            """
            {
                "operation":"unknown",
                "value1":"10",
                "value2":"20"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenPercentOfDivisionByZero()
    {
        var input =
            """
            {
                "operation":"percent-of",
                "value1":"10",
                "value2":"0"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenChangeOldValueIsZero()
    {
        var input =
            """
            {
                "operation":"change",
                "value1":"0",
                "value2":"100"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }
}