namespace WebApp.Api.Tests.Services;

public class DiffCheckerServiceTests
{
    private readonly DiffCheckerService _service = new();


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
    public void Execute_ShouldReturnDiff_WhenLinesAreChanged()
    {
        var input =
            """
            {
                "left":"Hello\nWorld\nFoo",
                "right":"Hello\nWorld!\nBar"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains("  Hello", result);
        Assert.Contains("- World", result);
        Assert.Contains("+ World!", result);
        Assert.Contains("- Foo", result);
        Assert.Contains("+ Bar", result);
    }


    [Fact]
    public void Execute_ShouldReturnSameLines_WhenTextsAreEqual()
    {
        var input =
            """
            {
                "left":"Hello\nWorld",
                "right":"Hello\nWorld"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains("  Hello", result);
        Assert.Contains("  World", result);

        Assert.DoesNotContain("- Hello", result);
        Assert.DoesNotContain("+ Hello", result);
    }

    [Fact]
    public void Execute_ShouldShowAddedLines()
    {
        var input =
            """
            {
                "left":"Hello",
                "right":"Hello\nWorld"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains("  Hello", result);
        Assert.Contains("+ World", result);
    }


    [Fact]
    public void Execute_ShouldShowRemovedLines()
    {
        var input =
            """
            {
                "left":"Hello\nWorld",
                "right":"Hello"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains("  Hello", result);
        Assert.Contains("- World", result);
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
    public void Execute_ShouldThrow_WhenJsonIsEmptyObject()
    {
        var input =
            """
            {
            }
            """;


        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Fact]
    public void Execute_ShouldThrow_WhenBothTextsAreEmpty()
    {
        var input =
            """
            {
                "left":"",
                "right":""
            }
            """;


        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Fact]
    public void Execute_ShouldHandleWindowsLineEndings()
    {
        var input =
            """
            {
                "left":"Hello\r\nWorld",
                "right":"Hello\r\nWorld!"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains("  Hello", result);
        Assert.Contains("- World", result);
        Assert.Contains("+ World!", result);
    }


    [Fact]
    public void Execute_ShouldHandleDifferentPropertyCases()
    {
        var input =
            """
            {
                "left":"Hello",
                "right":"World"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains("- Hello", result);
        Assert.Contains("+ World", result);
    }
}