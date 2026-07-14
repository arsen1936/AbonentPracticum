namespace WebApp.Api.Tests.Services;

public class NumberConverterServiceTests
{
    private readonly NumberConverterService _service = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Execute_ShouldReturnZero_WhenInputIsEmpty(string? input)
    {
        var result = _service.Execute(input!);
        Assert.Equal("0", result);
    }

    [Theory]
    [InlineData(
        """
        {
          "value":"1010",
          "fromBase":"bin",
          "toBase":"dec"
        }
        """,
        "10")]
    [InlineData(
        """
        {
          "value":"10",
          "fromBase":"dec",
          "toBase":"bin"
        }
        """,
        "1010")]
    [InlineData(
        """
        {
          "value":"255",
          "fromBase":"dec",
          "toBase":"hex"
        }
        """,
        "ff")]
    [InlineData(
        """
        {
          "value":"FF",
          "fromBase":"hex",
          "toBase":"dec"
        }
        """,
        "255")]
    [InlineData(
        """
        {
          "value":"17",
          "fromBase":"oct",
          "toBase":"dec"
        }
        """,
        "15")]
    [InlineData(
        """
        {
          "value":"15",
          "fromBase":"dec",
          "toBase":"oct"
        }
        """,
        "17")]
    public void Execute_ShouldConvertNumber(string input, string expected)
    {
        var result = _service.Execute(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenNumberIsInvalid()
    {
        var input = """
        {
          "value":"102",
          "fromBase":"bin",
          "toBase":"dec"
        }
        """;
        var ex = Assert.Throws<Exception>(() => _service.Execute(input));
        Assert.Equal("Неправильно введено число", ex.Message);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenSourceBaseIsUnknown()
    {
        var input = """
        {
          "value":"10",
          "fromBase":"abc",
          "toBase":"dec"
        }
        """;
        var ex = Assert.Throws<Exception>(() => _service.Execute(input));
        Assert.Equal("Неизвестная система счисления: 'abc'", ex.Message);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenTargetBaseIsUnknown()
    {
        var input = """
        {
          "value":"10",
          "fromBase":"dec",
          "toBase":"abc"
        }
        """;
        var ex = Assert.Throws<Exception>(() => _service.Execute(input));
        Assert.Equal("Неизвестная система счисления: 'abc'", ex.Message);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenJsonIsInvalid()
    {
        var input = "{ invalid json }";
        Assert.Throws<Exception>(() => _service.Execute(input));
    }
}