namespace WebApp.Api.Tests.Services;

public class Base64ConverterServiceTests
{
    private readonly Base64ConverterService _service = new();

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
    public void Execute_ShouldEncodeToBase64()
    {
        var input =
            "encode\n" +
            "Hello, World!";

        var expected = "SGVsbG8sIFdvcmxkIQ==";

        var result = _service.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldDecodeFromBase64()
    {
        var input =
            "decode\n" +
            "SGVsbG8sIFdvcmxkIQ==";

        var expected = "Hello, World!";

        var result = _service.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldEncodeUnicodeString()
    {
        var input =
            "encode\n" +
            "Привет 🌍";

        var expected = "0J/RgNC40LLQtdGCIPCfjI0=";

        var result = _service.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldDecodeUnicodeString()
    {
        var input =
            "decode\n" +
            "0J/RgNC40LLQtdGCIPCfjI0=";

        var expected = "Привет 🌍";

        var result = _service.Execute(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("encode")]
    [InlineData("decode\n")]
    [InlineData("encode\nhello\nextra")]
    public void Execute_ShouldThrow_WhenInputFormatIsInvalid(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenModeIsInvalid()
    {
        var input =
            "convert\n" +
            "Hello";

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenBase64IsInvalid()
    {
        var input =
            "decode\n" +
            "not-base64!";

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Theory]
    [InlineData("SGVsbG8*")]
    [InlineData("SGVsbG8===")]
    [InlineData("!!!")]
    [InlineData("abc")]
    public void Execute_ShouldThrow_WhenBase64FormatIsIncorrect(string base64)
    {
        var input = $"decode\n{base64}";

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }
}