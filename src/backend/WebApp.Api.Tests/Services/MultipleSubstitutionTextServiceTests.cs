namespace WebApp.Api.Tests.Services;

public class MultipleSubstitutionTextServiceTests
{
    private readonly MultipleSubstitutionTextService _service = new();

    [Fact]
    public void Execute_ShouldReplaceSingleWord()
    {
        var input = """
        {
          "text": "hello world",
          "replacements": {
            "hello": "hi"
          }
        }
        """;
        var result = _service.Execute(input);
        Assert.Equal("hi world", result);
    }

    [Fact]
    public void Execute_ShouldReplaceMultipleWords()
    {
        var input = """
        {
          "text": "cat and dog",
          "replacements": {
            "cat": "mouse",
            "dog": "bird"
          }
        }
        """;
        var result = _service.Execute(input);
        Assert.Equal("mouse and bird", result);
    }

    [Fact]
    public void Execute_ShouldPreferLongestMatch()
    {
        var input = """
        {
          "text": "abc",
          "replacements": {
            "a": "1",
            "abc": "2"
          }
        }
        """;
        var result = _service.Execute(input);
        Assert.Equal("2", result);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenReplacementsAreEmpty()
    {
        var input = """
        {
          "text": "hello",
          "replacements": {}
        }
        """;
        var ex = Assert.Throws<ArgumentException>(() => _service.Execute(input));
        Assert.Equal("Добавьте хотя бы одну замену.", ex.Message);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenTextIsEmpty()
    {
        var input = """
        {
          "text": "",
          "replacements": {
            "a": "b"
          }
        }
        """;
        var ex = Assert.Throws<ArgumentException>(() => _service.Execute(input));
        Assert.Equal("Поле text обязательно.", ex.Message);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenJsonIsInvalid()
    {
        var input = "{ invalid json }";
        var ex = Assert.Throws<ArgumentException>(() => _service.Execute(input));
        Assert.Equal("Некорректный JSON.", ex.Message);
    }
    
    [Fact]
    public void Execute_ShouldThrow_WhenReplacementsIsNull()
    {
        var input = """
                    {
                      "text": "hello",
                      "replacements": null
                    }
                    """;
        var ex = Assert.Throws<ArgumentException>(() => _service.Execute(input));
        Assert.Equal("Поле replacements обязательно.", ex.Message);
    }
}