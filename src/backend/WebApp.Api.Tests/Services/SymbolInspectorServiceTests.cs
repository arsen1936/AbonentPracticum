namespace WebApp.Api.Tests.Services;

public class SymbolInspectorServiceTests
{
    private readonly SymbolInspectorService _service = new();

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
    public void Execute_ShouldEncodeAsciiString()
    {
        var input =
            """
            encode
            Hello
            """;

        var result = _service.Execute(input);

        Assert.Equal("U+0048 U+0065 U+006C U+006C U+006F", result);
    }

    [Fact]
    public void Execute_ShouldEncodeUnicodeString()
    {
        var input =
            """
            encode
            Привет!
            """;

        var result = _service.Execute(input);

        Assert.Equal(
            "U+041F U+0440 U+0438 U+0432 U+0435 U+0442 U+0021",
            result);
    }

    [Fact]
    public void Execute_ShouldDecodeAsciiCodes()
    {
        var input =
            """
            decode
            0048 0065 006C 006C 006F
            """;

        var result = _service.Execute(input);

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Execute_ShouldDecodeUnicodeCodes()
    {
        var input =
            """
            decode
            041F 0440 0438 0432 0435 0442
            """;

        var result = _service.Execute(input);

        Assert.Equal("Привет", result);
    }

    [Theory]
    [InlineData("encode")]
    [InlineData("encode\nHello\nWorld")]
    public void Execute_ShouldThrow_WhenInputFormatIsInvalid(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenModeIsInvalid()
    {
        var input =
            """
            convert
            Hello
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Theory]
    [InlineData("ZZZZ")]
    [InlineData("0048 ZZZZ")]
    [InlineData("123G")]
    [InlineData("U+0048")]
    public void Execute_ShouldThrow_WhenHexCodeIsInvalid(string codes)
    {
        var input = $"decode\n{codes}";

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldEncodeSpaceCharacter()
    {
        var input =
            """
            encode
             
            """;

        var result = _service.Execute(input);

        Assert.Equal("U+0020", result);
    }

    [Fact]
    public void Execute_ShouldDecodeSpaceCharacter()
    {
        var input =
            """
            decode
            0020
            """;

        var result = _service.Execute(input);

        Assert.Equal(" ", result);
    }
}