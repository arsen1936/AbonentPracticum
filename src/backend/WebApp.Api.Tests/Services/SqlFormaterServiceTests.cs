namespace WebApp.Api.Tests.Services;

public class SqlFormaterServiceTests
{
    private readonly SqlFormaterService _service = new();


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
    [InlineData("beautify")]
    [InlineData("minify\n")]
    [InlineData("beautify\n")]
    public void Execute_ShouldThrow_WhenSqlIsMissing(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Fact]
    public void Execute_ShouldThrow_WhenModeIsInvalid()
    {
        var input =
            """
            format
            SELECT id FROM users
            """;

        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }


    [Fact]
    public void Execute_ShouldBeautifySimpleSelect()
    {
        var input =
            """
            beautify
            select id,name from users where age>18
            """;

        var result = _service.Execute(input);

        Assert.Contains("SELECT", result);
        Assert.Contains("FROM users", result);
        Assert.Contains("WHERE age>18", result);
    }


    [Fact]
    public void Execute_ShouldConvertKeywordsToUpperCase()
    {
        var input =
            """
            beautify
            select id from users where name='Ivan'
            """;

        var result = _service.Execute(input);

        Assert.DoesNotContain("select", result);
        Assert.DoesNotContain("from", result);
        Assert.DoesNotContain("where", result);

        Assert.Contains("SELECT", result);
        Assert.Contains("FROM", result);
        Assert.Contains("WHERE", result);
    }


    [Fact]
    public void Execute_ShouldFormatColumnsWithIndent()
    {
        var input =
            """
            beautify
            SELECT id,name,email FROM users
            """;

        var result = _service.Execute(input);

        Assert.Contains(
            "id,\n",
            result
        );

        Assert.Contains(
            "name,\n",
            result
        );

        Assert.Contains(
            "email",
            result
        );
    }


    [Fact]
    public void Execute_ShouldBeautifyJoinQuery()
    {
        var input =
            """
            beautify
            select u.id,o.id from users u join orders o on u.id=o.user_id
            """;

        var result = _service.Execute(input);

        Assert.Contains("SELECT", result);
        Assert.Contains("JOIN", result);
        Assert.Contains("FROM", result);
    }


    [Fact]
    public void Execute_ShouldFormatNestedQuery()
    {
        var input =
            """
            beautify
            select id from users where id in(select user_id from orders)
            """;

        var result = _service.Execute(input);

        Assert.Contains("(", result);
        Assert.Contains(")", result);
        Assert.Contains("SELECT", result);
        Assert.Contains("FROM orders", result);
    }


    [Fact]
    public void Execute_ShouldMinifySql()
    {
        var input =
            """
            minify
            SELECT
                id,
                name
            FROM users
            WHERE age > 18
            """;

        var result = _service.Execute(input);

        Assert.Equal(
            "SELECT id,name FROM users WHERE age > 18",
            result
        );
    }


    [Fact]
    public void Execute_ShouldRemoveExtraSpaces()
    {
        var input =
            """
            minify
            SELECT     id     FROM     users
            """;

        var result = _service.Execute(input);

        Assert.Equal(
            "SELECT id FROM users",
            result
        );
    }


    [Fact]
    public void Execute_ShouldKeepSemicolonInMinify()
    {
        var input =
            """
            minify
            SELECT id FROM users;
            """;

        var result = _service.Execute(input);

        Assert.Equal(
            "SELECT id FROM users;",
            result
        );
    }


    [Fact]
    public void Execute_ShouldFormatGroupAndOrderBy()
    {
        var input =
            """
            beautify
            select city,count(id) from users group by city order by city
            """;

        var result = _service.Execute(input);

        Assert.Contains("GROUP BY", result);
        Assert.Contains("ORDER BY", result);
    }


    [Theory]
    [InlineData("SELECT id FROM users")]
    [InlineData("select name from table")]
    [InlineData("SeLeCt id FrOm users")]
    public void Execute_ShouldWorkWithDifferentKeywordCases(string sql)
    {
        var input = $"beautify\n{sql}";

        var result = _service.Execute(input);

        Assert.Contains("SELECT", result);
        Assert.Contains("FROM", result);
    }
}