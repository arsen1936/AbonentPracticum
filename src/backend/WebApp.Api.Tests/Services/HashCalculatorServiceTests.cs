namespace WebApp.Api.Tests.Services;

public class HashCalculatorServiceTests
{
    private readonly HashCalculatorService _service = new();
    
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
    [InlineData("md5\nhello", "5D41402ABC4B2A76B9719D911017C592")]
    [InlineData("sha1\nhello", "AAF4C61DDCC5E8A2DABEDE0F3B482CD9AEA9434D")]
    [InlineData("sha256\nhello", "2CF24DBA5FB0A30E26E83B2AC5B9E29E1B161E5C1FA7425E73043362938B9824")]
    [InlineData("sha512\nhello", "9B71D224BD62F3785D96D46AD3EA3D73319BFBC2890CAADAE2DFF72519673CA72323C3D99BA5C11D7C7ACC6E14B8C5DA0C4663475C2E5C3ADEF46F73BCDEC043")]
    public void Execute_ShouldCalculateHash(string input, string expected)
    {
        var result = _service.Execute(input);
        Assert.Equal(expected.Replace("\n", ""), result);
    }

    [Fact]
    public void Execute_ShouldIgnoreMethodCase()
    {
        var result = _service.Execute("ShA256\nhello");
        
        Assert.Equal(
            "2CF24DBA5FB0A30E26E83B2AC5B9E29E1B161E5C1FA7425E73043362938B9824",
            result);
    }

    [Theory]
    [InlineData("md5")]
    [InlineData("md5\nhello\nextra")]
    [InlineData("only one line")]
    public void Execute_ShouldThrow_WhenInputFormatIsInvalid(string input)
    {
        Assert.Throws<Exception>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenAlgorithmIsNotSupported()
    {
        Assert.Throws<Exception>(() => _service.Execute("sha999\nhello"));
    }
}