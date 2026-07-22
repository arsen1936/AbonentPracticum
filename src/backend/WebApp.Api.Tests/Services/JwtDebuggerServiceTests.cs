namespace WebApp.Api.Tests.Services;

public class JwtDebuggerServiceTests
{
    private readonly JwtDebuggerService _service = new();


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Execute_ShouldThrow_WhenInputIsEmpty(string? input)
    {
        Assert.Throws<ArgumentException>(
            () => _service.Execute(input!)
        );
    }


    [Theory]
    [InlineData("decode")]
    [InlineData("generate")]
    public void Execute_ShouldThrow_WhenDataIsMissing(string mode)
    {
        var input = mode;


        Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );
    }


    [Fact]
    public void Execute_ShouldThrow_WhenModeIsInvalid()
    {
        var input =
            """
            test
            data
            """;


        Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );
    }


    [Fact]
    public void Execute_ShouldGenerateJwtToken()
    {
        var input =
            """
            generate
            {
                "sub":"12345",
                "name":"Иван",
                "role":"admin"
            }
            """;


        var result = _service.Execute(input);


        Assert.NotEmpty(result);
        Assert.Equal(3, result.Split('.').Length);
    }


    [Fact]
    public void Execute_ShouldThrow_WhenGenerateJsonIsInvalid()
    {
        var input =
            """
            generate
            {
                "sub":
            }
            """;


        Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );
    }


    [Fact]
    public void Execute_ShouldThrow_WhenClaimsAreMissing()
    {
        var input =
            """
            generate
            null
            """;


        Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );
    }


    [Fact]
    public void Execute_ShouldDecodeJwtToken()
    {
        var generateInput =
            """
            generate
            {
                "sub":"12345",
                "name":"Ivan"
            }
            """;


        var token = _service.Execute(generateInput);


        var decodeInput =
            $"""
            decode
            {token}
            """;


        var result = _service.Execute(decodeInput);


        Assert.Contains(
            "=== HEADER ===",
            result
        );

        Assert.Contains(
            "=== PAYLOAD ===",
            result
        );

        Assert.Contains(
            "sub",
            result
        );

        Assert.Contains(
            "12345",
            result
        );
    }


    [Theory]
    [InlineData("abc")]
    [InlineData("abc.def")]
    [InlineData("abc.def.xyz.test")]
    public void Execute_ShouldThrow_WhenJwtStructureIsInvalid(string token)
    {
        var input =
            $"""
            decode
            {token}
            """;


        Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );
    }


    [Fact]
    public void Execute_ShouldDecodeExpiredToken()
    {
        var header =
            "eyJhbGciOiJIUzI1NiJ9";

        var payload =
            "eyJleHAiOjF9";

        var token =
            $"{header}.{payload}.signature";


        var input =
            $"""
            decode
            {token}
            """;


        var result = _service.Execute(input);


        Assert.Contains(
            "Токен истёк.",
            result
        );
    }


    [Theory]
    [InlineData("DECODE")]
    [InlineData("Decode")]
    [InlineData("decode")]
    public void Execute_ShouldIgnoreModeCase(string mode)
    {
        var generateInput =
            """
            generate
            {
                "sub":"1"
            }
            """;


        var token = _service.Execute(generateInput);


        var input =
            $"""
            {mode}
            {token}
            """;


        var result = _service.Execute(input);


        Assert.Contains(
            "=== HEADER ===",
            result
        );
    }


    [Fact]
    public void Execute_ShouldGenerateDifferentTokens_WhenClaimsAreDifferent()
    {
        var first =
            """
            generate
            {
                "sub":"1"
            }
            """;


        var second =
            """
            generate
            {
                "sub":"2"
            }
            """;


        var token1 = _service.Execute(first);
        var token2 = _service.Execute(second);


        Assert.NotEqual(
            token1,
            token2
        );
    }
}