namespace WebApp.Api.Tests.Services;

public class CountingCharactersServiceTests
{
    private readonly CountingCharactersService _service = new();

    [Fact]
    public void Execute_ShouldReturnNumberOfCharactersWithSpaces()
    {
        var result = _service.Execute("In this   row 22 symbols");
        Assert.Equal("Символов (с пробелами): 24", result.Split("\n")[0]);
    }
    
    [Fact]
    public void Execute_ShouldReturnNumberOfCharactersWithoutSpaces()
    {
        var result = _service.Execute("In           this row 18 symbols");
        Assert.Equal("Символов (без пробелов): 18", result.Split("\n")[1]);
    }
    
    [Fact]
    public void Execute_ShouldReturnNumberOfWords()
    {
        var result = _service.Execute("In this          row 5 words");
        Assert.Equal("Слов: 5", result.Split("\n")[2]);
    }

    [Fact]
    public void Execute_ShouldReturnNumberOfLines()
    {
        var result = _service.Execute("In this\n row \n3 words");
        Assert.Equal("Строк: 3", result.Split("\n")[3]);
    }

    [Fact]
    public void Execute_ShouldReturnNumberOfSentences()
    {
        var result = _service.Execute("In this!!! row. 3 sentences?");
        Assert.Equal("Предложений: 3", result.Split("\n")[4]);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenInputIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _service.Execute(null!));
    }
}