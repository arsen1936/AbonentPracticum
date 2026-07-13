namespace WebApp.Api.Tests.Services;

public class TextToListServiceTests
{
    private readonly TextToListService _service = new();

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

    [Fact]
    public void Execute_ShouldGenerateJavaList()
    {
        var input = "java\napple\nbanana";
        var expected =
            "List<String> items = new ArrayList<>(Arrays.asList(\n" +
            "\"apple\",\n" +
            "\"banana\"));";

        var result = _service.Execute(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldGenerateCSharpList()
    {
        var input = "csharp\napple\nbanana";
        var expected =
            "var items = new List<string>\n{\n" +
            "    \"apple\",\n" +
            "    \"banana\"\n};";

        var result = _service.Execute(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldGenerateJavaScriptArray()
    {
        var input = "js\napple\nbanana";
        var expected =
            "const items = [\n" +
            "\"apple\",\n" +
            "\"banana\"\n];";

        var result = _service.Execute(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldGeneratePythonList()
    {
        var input = "python\napple\nbanana";
        var expected =
            "items = [\n" +
            "    \"apple\",\n" +
            "    \"banana\"\n]";

        var result = _service.Execute(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldGenerateSqlInList()
    {
        var input = "sql\napple\nbanana";
        var expected =
            "IN (\n" +
            "    'apple',\n" +
            "    'banana'\n)";

        var result = _service.Execute(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("JAVA")]
    [InlineData("Java")]
    [InlineData("CsHaRp")]
    [InlineData("PYTHON")]
    public void Execute_ShouldIgnoreLanguageCase(string language)
    {
        var input = $"{language}\napple";
        var result = _service.Execute(input);
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("java")]
    [InlineData("python\n")]
    public void Execute_ShouldThrow_WhenInputFormatIsInvalid(string input)
    {
        Assert.Throws<Exception>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenLanguageIsInvalid()
    {
        var input = "cpp\napple\nbanana";
        Assert.Throws<Exception>(() => _service.Execute(input));
    }
}