namespace WebApp.Api.Tests.Services;

public class CsvToJsonConverterServiceTests
{
    private readonly CsvToJsonConverterService _service = new();

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
    [InlineData("csv-to-json")]
    [InlineData("json-to-csv")]
    [InlineData("csv-to-json\n")]
    public void Execute_ShouldThrow_WhenInputFormatIsInvalid(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenModeIsInvalid()
    {
        var input =
            """
            xml
            test
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldConvertCsvToJson()
    {
        var input =
            """
            csv-to-json
            name,age,city
            Иван,25,Москва
            Анна,30,Питер
            """;

        var result = _service.Execute(input);

        Assert.Contains("\"name\": \"Иван\"", result);
        Assert.Contains("\"age\": \"25\"", result);
        Assert.Contains("\"city\": \"Москва\"", result);
        Assert.Contains("\"name\": \"Анна\"", result);
    }

    [Fact]
    public void Execute_ShouldConvertJsonToCsv()
    {
        var input =
            """
            json-to-csv
            [
              {
                "name":"Иван",
                "age":"25"
              },
              {
                "name":"Анна",
                "age":"30"
              }
            ]
            """;

        var expected =
            """
            name,age
            Иван,25
            Анна,30
            """;

        var result = _service.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldHandleQuotedCsvFields()
    {
        var input =
            """
            csv-to-json
            name,comment
            Иван,"Привет, мир"
            """;

        var result = _service.Execute(input);

        Assert.Contains("\"comment\": \"Привет, мир\"", result);
    }

    [Fact]
    public void Execute_ShouldEscapeQuotes_WhenConvertingToCsv()
    {
        var input =
            """
            json-to-csv
            [
              {
                "text":"Он сказал \"Привет\""
              }
            ]
            """;

        var result = _service.Execute(input);

        Assert.Contains("\"Он сказал \"\"Привет\"\"\"", result);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenCsvColumnsCountIsInvalid()
    {
        var input =
            """
            csv-to-json
            name,age
            Иван
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenCsvContainsOnlyHeader()
    {
        var input =
            """
            csv-to-json
            name,age
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Theory]
    [InlineData("{")]
    [InlineData("abc")]
    [InlineData("{}")]
    public void Execute_ShouldThrow_WhenJsonIsInvalid(string json)
    {
        var input = $"json-to-csv\n{json}";

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }
}