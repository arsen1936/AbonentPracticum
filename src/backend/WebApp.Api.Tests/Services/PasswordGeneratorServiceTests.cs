namespace WebApp.Api.Tests.Services;

public class PasswordGeneratorServiceTests
{
    private readonly PasswordGeneratorService _service = new();

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
    [InlineData("{")]
    [InlineData("abc")]
    [InlineData("[1,2,3]")]
    public void Execute_ShouldThrow_WhenJsonIsInvalid(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(257)]
    public void Execute_ShouldThrow_WhenLengthIsOutOfRange(int length)
    {
        var input =
            $$"""
            {
                "length": {{length}},
                "useUpper": true,
                "useLower": true,
                "useDigits": true,
                "useSymbols": true
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenNoCharacterSetsSelected()
    {
        var input =
            """
            {
                "length":16,
                "useUpper":false,
                "useLower":false,
                "useDigits":false,
                "useSymbols":false
            }
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldGeneratePasswordWithRequestedLength()
    {
        var input =
            """
            {
                "length":20,
                "useUpper":true,
                "useLower":true,
                "useDigits":true,
                "useSymbols":true
            }
            """;

        var result = _service.Execute(input);

        Assert.Equal(20, result.Length);
    }

    [Fact]
    public void Execute_ShouldGeneratePasswordWithUppercaseOnly()
    {
        var input =
            """
            {
                "length":10,
                "useUpper":true,
                "useLower":false,
                "useDigits":false,
                "useSymbols":false
            }
            """;

        var result = _service.Execute(input);

        Assert.Matches("^[A-Z]{10}$", result);
    }

    [Fact]
    public void Execute_ShouldGeneratePasswordWithLowercaseOnly()
    {
        var input =
            """
            {
                "length":10,
                "useUpper":false,
                "useLower":true,
                "useDigits":false,
                "useSymbols":false
            }
            """;

        var result = _service.Execute(input);

        Assert.Matches("^[a-z]{10}$", result);
    }

    [Fact]
    public void Execute_ShouldGeneratePasswordWithDigitsOnly()
    {
        var input =
            """
            {
                "length":10,
                "useUpper":false,
                "useLower":false,
                "useDigits":true,
                "useSymbols":false
            }
            """;

        var result = _service.Execute(input);

        Assert.Matches("^[0-9]{10}$", result);
    }

    [Fact]
    public void Execute_ShouldContainAllSelectedCharacterTypes()
    {
        var input =
            """
            {
                "length":30,
                "useUpper":true,
                "useLower":true,
                "useDigits":true,
                "useSymbols":true
            }
            """;

        var result = _service.Execute(input);

        Assert.Contains(result, char.IsUpper);
        Assert.Contains(result, char.IsLower);
        Assert.Contains(result, char.IsDigit);
        Assert.Contains(result, c => !char.IsLetterOrDigit(c));
    }

    [Fact]
    public void Execute_ShouldGenerateDifferentPasswords()
    {
        var input =
            """
            {
                "length":16,
                "useUpper":true,
                "useLower":true,
                "useDigits":true,
                "useSymbols":true
            }
            """;

        var password1 = _service.Execute(input);
        var password2 = _service.Execute(input);

        Assert.NotEqual(password1, password2);
    }
}