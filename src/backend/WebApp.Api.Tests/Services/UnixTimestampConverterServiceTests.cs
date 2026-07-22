namespace WebApp.Api.Tests.Services;

public class UnixTimestampConverterServiceTests
{
    private readonly UnixTimestampConverterService _service = new();

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
    public void Execute_ShouldConvertUnixTimestampToDate()
    {
        var input =
            "to-date\n" +
            "1785963600";

        var expected = "05.08.2026 21:00:00 (UTC)";

        var result = _service.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldConvertDateToUnixTimestamp()
    {
        var input =
            "to-timestamp\n" +
            "2026-08-05T21:00:00Z";

        var expected = "1785963600";

        var result = _service.Execute(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("to-date")]
    [InlineData("to-timestamp\n")]
    [InlineData("to-date\n123\n456")]
    public void Execute_ShouldThrow_WhenInputFormatIsInvalid(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenModeIsInvalid()
    {
        var input =
            "convert\n" +
            "1785963600";

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenTimestampIsNotNumber()
    {
        var input =
            "to-date\n" +
            "abc";

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenTimestampIsOutOfRange()
    {
        var input =
            "to-date\n" +
            long.MaxValue;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenDateIsInvalid()
    {
        var input =
            "to-timestamp\n" +
            "32.13.2026";

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Theory]
    [InlineData("2026-08-05T05:40:00Z")]
    [InlineData("05.08.2026 05:40:00")]
    [InlineData("2026-08-05 05:40:00")]
    public void Execute_ShouldParseDifferentDateFormats(string date)
    {
        var input = $"to-timestamp\n{date}";

        var result = _service.Execute(input);

        Assert.NotNull(result);
    }
}