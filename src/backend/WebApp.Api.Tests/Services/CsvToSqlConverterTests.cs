namespace WebApp.Api.Tests.Services;

public class CsvToSqlConverterTests
{
    private readonly CsvToSqlConverter _service = new();

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
    [InlineData("employees")]
    [InlineData("employees\n")]
    public void Execute_ShouldThrow_WhenInputFormatIsInvalid(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenTableNameIsEmpty()
    {
        var input =
            """
            
            name,age
            Ivan,20
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldConvertCsvToSql()
    {
        var input =
            """
            employees
            name,age,city
            Иван,25,Москва
            Анна,30,Питер
            """;

        var result = _service.Execute(input);

        var expected =
            """
            CREATE TEMP TABLE employees (
                name TEXT,
                age INT,
                city TEXT
            );
            INSERT INTO employees VALUES ('Иван', 25, 'Москва');
            INSERT INTO employees VALUES ('Анна', 30, 'Питер');
            """;

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldDetectIntegerType()
    {
        var input =
            """
            users
            id
            1
            2
            3
            """;

        var result = _service.Execute(input);

        Assert.Contains("id INT", result);
    }

    [Fact]
    public void Execute_ShouldDetectRealType()
    {
        var input =
            """
            products
            price
            10.5
            20.75
            """;

        var result = _service.Execute(input);

        Assert.Contains("price REAL", result);
    }

    [Fact]
    public void Execute_ShouldDetectTextType()
    {
        var input =
            """
            cities
            name
            Москва
            Питер
            """;

        var result = _service.Execute(input);

        Assert.Contains("name TEXT", result);
    }

    [Fact]
    public void Execute_ShouldEscapeSingleQuotesInText()
    {
        var input =
            """
            books
            title
            Harry's book
            """;

        var result = _service.Execute(input);

        Assert.Contains("'Harry''s book'", result);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenColumnsCountDoesNotMatch()
    {
        var input =
            """
            employees
            name,age,city
            Иван,25
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenCsvContainsOnlyHeader()
    {
        var input =
            """
            employees
            name,age
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldGenerateMultipleInsertStatements()
    {
        var input =
            """
            users
            name,age
            Ivan,20
            Anna,30
            Bob,40
            """;

        var result = _service.Execute(input);

        Assert.Equal(3, result.Split("INSERT INTO").Length - 1);
    }
}