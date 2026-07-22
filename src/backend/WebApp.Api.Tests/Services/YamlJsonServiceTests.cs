namespace WebApp.Api.Tests.Services;

public class YamlJsonServiceTests
{
    private readonly YamlJsonService _service = new();


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
    public void Execute_ShouldThrow_WhenContentIsMissing()
    {
        var input =
            """
            {
                "mode":"json2yaml",
                "format":"json"
            }
            """;


        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Fact]
    public void Execute_ShouldConvertJsonToYaml()
    {
        var input =
            """
            {
                "mode":"json2yaml",
                "format":"json",
                "content":"{\"name\":\"Иван\",\"age\":25}"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains("name: Иван", result);
        Assert.Contains("age: 25", result);
    }


    [Fact]
    public void Execute_ShouldConvertYamlToJson()
    {
        var input =
            """
            {
                "mode":"yaml2json",
                "format":"yaml",
                "content":"name: Иван\nage: 25"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains("\"name\": \"Иван\"", result);
        Assert.Contains("\"age\":", result);
        Assert.Contains("25", result);
    }


    [Fact]
    public void Execute_ShouldConvertYamlArrayToJson()
    {
        var input =
            """
            {
                "mode":"yaml2json",
                "format":"yaml",
                "content":"skills:\n  - C#\n  - SQL"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains("\"skills\":", result);
        Assert.Contains("\"C#\"", result);
        Assert.Contains("\"SQL\"", result);
    }


    [Fact]
    public void Execute_ShouldValidateCorrectJson()
    {
        var input =
            """
            {
                "mode":"validate",
                "format":"json",
                "content":"{\"name\":\"Ivan\"}"
            }
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "JSON корректный.",
            result
        );
    }


    [Fact]
    public void Execute_ShouldValidateCorrectYaml()
    {
        var input =
            """
            {
                "mode":"validate",
                "format":"yaml",
                "content":"name: Ivan\nage: 20"
            }
            """;


        var result = _service.Execute(input);


        Assert.Equal(
            "YAML корректный.",
            result
        );
    }


    [Fact]
    public void Execute_ShouldThrow_WhenJsonIsInvalid()
    {
        var input =
            """
            {
                "mode":"json2yaml",
                "format":"json",
                "content":"{\"name\":}"
            }
            """;


        Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );
    }


    [Fact]
    public void Execute_ShouldThrow_WhenYamlIsInvalid()
    {
        var input =
            """
            {
                "mode":"yaml2json",
                "format":"yaml",
                "content":"name: Ivan\n  age: 25"
            }
            """;


        var exception = Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );


        Assert.Contains(
            "Ошибка YAML",
            exception.Message
        );
    }


    [Theory]
    [InlineData("{")]
    [InlineData("abc")]
    [InlineData("[1,2,3]")]
    public void Execute_ShouldThrow_WhenRequestJsonIsInvalid(string input)
    {
        Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );
    }


    [Fact]
    public void Execute_ShouldThrow_WhenModeIsInvalid()
    {
        var input =
            """
            {
                "mode":"convert",
                "format":"json",
                "content":"{\"name\":\"Ivan\"}"
            }
            """;


        Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );
    }


    [Fact]
    public void Execute_ShouldThrow_WhenValidateFormatIsInvalid()
    {
        var input =
            """
            {
                "mode":"validate",
                "format":"xml",
                "content":"<name>Ivan</name>"
            }
            """;


        Assert.Throws<ArgumentException>(
            () => _service.Execute(input)
        );
    }


    [Theory]
    [InlineData("JSON2YAML")]
    [InlineData("Json2Yaml")]
    [InlineData("json2yaml")]
    public void Execute_ShouldIgnoreModeCase(string mode)
    {
        var input =
            $$"""
            {
                "mode":"{{mode}}",
                "format":"json",
                "content":"{\"name\":\"Ivan\"}"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains(
            "name: Ivan",
            result
        );
    }


    [Fact]
    public void Execute_ShouldKeepNestedJsonStructure_WhenConvertingToYaml()
    {
        var input =
            """
            {
                "mode":"json2yaml",
                "format":"json",
                "content":"{\"user\":{\"name\":\"Ivan\",\"age\":25}}"
            }
            """;


        var result = _service.Execute(input);


        Assert.Contains(
            "user:",
            result
        );

        Assert.Contains(
            "name: Ivan",
            result
        );

        Assert.Contains(
            "age: 25",
            result
        );
    }
}