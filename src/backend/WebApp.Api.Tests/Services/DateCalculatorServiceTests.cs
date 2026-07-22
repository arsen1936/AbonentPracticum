namespace WebApp.Api.Tests.Services;

public class DateCalculatorServiceTests
{
    private readonly DateCalculatorService _service = new();

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
    public void Execute_ShouldCalculateDateDifference()
    {
        var input =
            """
            {
                "operation":"diff",
                "date1":"2026-01-01",
                "date2":"2026-06-28"
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal("Разница: 178 дней (0 лет, 5 месяцев, 27 дней)", result);
    }

    [Fact]
    public void Execute_ShouldCalculateDateDifference_WhenDatesAreReversed()
    {
        var input =
            """
            {
                "operation":"diff",
                "date1":"2026-06-28",
                "date2":"2026-01-01"
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal("Разница: 178 дней (0 лет, 5 месяцев, 27 дней)", result);
    }

    [Theory]
    [InlineData(30, "days", "2026-01-31")]
    [InlineData(2, "months", "2026-03-01")]
    [InlineData(5, "years", "2031-01-01")]
    public void Execute_ShouldAddInterval(int amount, string unit, string expected)
    {
        var input =
            $$"""
            {
                "operation":"add",
                "date1":"2026-01-01",
                "amount":{{amount}},
                "unit":"{{unit}}"
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal(expected, result);
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
    public void Execute_ShouldThrow_WhenOperationIsInvalid()
    {
        var input =
            """
            {
                "operation":"test"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenAmountIsMissing()
    {
        var input =
            """
            {
                "operation":"add",
                "date1":"2026-01-01",
                "unit":"days"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenUnitIsMissing()
    {
        var input =
            """
            {
                "operation":"add",
                "date1":"2026-01-01",
                "amount":10
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenUnitIsInvalid()
    {
        var input =
            """
            {
                "operation":"add",
                "date1":"2026-01-01",
                "amount":10,
                "unit":"weeks"
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenDateFormatIsInvalid_ForDiff()
    {
        var input =
            """
            {
                "operation":"diff",
                "date1":"01.01.2026",
                "date2":"2026-06-28"
            }
            """;

        Assert.Throws<FormatException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenDateFormatIsInvalid_ForAdd()
    {
        var input =
            """
            {
                "operation":"add",
                "date1":"01.01.2026",
                "amount":10,
                "unit":"days"
            }
            """;

        Assert.Throws<FormatException>(() => _service.Execute(input));
    }
}