namespace WebApp.Api.Tests.Services;

public class EscapingStringsServiceTests
{
    private readonly EscapingStringsService _service = new();


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
    [InlineData("escape")]
    [InlineData("escape\n")]
    [InlineData("escape:html")]
    public void Execute_ShouldThrow_WhenModeIsMissing(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Fact]
    public void Execute_ShouldEscapeHtml()
    {
        var input =
            """
            escape:html
            <script>alert('XSS')</script>
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;",
            result
        );
    }


    [Fact]
    public void Execute_ShouldUnescapeHtml()
    {
        var input =
            """
            unescape:html
            &lt;script&gt;alert(&#x27;XSS&#x27;)&lt;/script&gt;
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "<script>alert('XSS')</script>",
            result
        );
    }


    [Fact]
    public void Execute_ShouldEscapeJson()
    {
        var input =
            """
            escape:json
            Hello "world"
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "Hello \\\"world\\\"",
            result
        );
    }


    [Fact]
    public void Execute_ShouldUnescapeJson()
    {
        var input =
            """
            unescape:json
            Hello \"world\"
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            @"Hello ""world""",
            result
        );
    }


    [Fact]
    public void Execute_ShouldEscapeSql()
    {
        var input =
            """
            escape:sql
            SELECT * FROM users WHERE name='Ivan'
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "SELECT * FROM users WHERE name=''Ivan''",
            result
        );
    }


    [Fact]
    public void Execute_ShouldUnescapeSql()
    {
        var input =
            """
            unescape:sql
            SELECT * FROM users WHERE name=''Ivan''
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "SELECT * FROM users WHERE name='Ivan'",
            result
        );
    }


    [Fact]
    public void Execute_ShouldEscapeUrl()
    {
        var input =
            """
            escape:url
            hello world/test
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "hello%20world%2Ftest",
            result
        );
    }


    [Fact]
    public void Execute_ShouldUnescapeUrl()
    {
        var input =
            """
            unescape:url
            hello%20world%2Ftest
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "hello world/test",
            result
        );
    }


    [Theory]
    [InlineData("escape:xml")]
    [InlineData("escape")]
    [InlineData("test:html")]
    [InlineData("escape:")]
    public void Execute_ShouldThrow_WhenModeIsInvalid(string mode)
    {
        var input =
            $"""
            {mode}
            test
            """;


        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Theory]
    [InlineData("escape:html")]
    [InlineData("escape:json")]
    [InlineData("escape:sql")]
    [InlineData("escape:url")]
    public void Execute_ShouldThrow_WhenTextIsEmpty(string mode)
    {
        var input =
            $"""
            {mode}

            """;


        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Theory]
    [InlineData("abc")]
    [InlineData("escape:html:test")]
    public void Execute_ShouldThrow_WhenModeFormatIsInvalid(string mode)
    {
        var input =
            $"""
            {mode}
            text
            """;


        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }
}