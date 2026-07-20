namespace WebApp.Api.Tests.Services;

public class StringSorterServiceTests
{
    private readonly StringSorterService _service = new();

    [Theory]
    [InlineData("asc\npear\napple\nbanana", "apple, banana, pear")]
    [InlineData("ASC\ncat\nbird\ndog", "bird, cat, dog")]
    public void Execute_ShouldSortAscending(string input, string expected)
    {
        var result = _service.Execute(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("desc\npear\napple\nbanana", "pear, banana, apple")]
    [InlineData("DESC\ncat\nbird\ndog", "dog, cat, bird")]
    public void Execute_ShouldSortDescending(string input, string expected)
    {
        var result = _service.Execute(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldRemoveDuplicates()
    {
        var input = "asc\nbanana\napple\nbanana\napple";
        var result = _service.Execute(input);
        Assert.Equal("apple, banana", result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("asc")]
    public void Execute_ShouldThrow_WhenInputFormatIsInvalid(string input)
    {
        Assert.Throws<Exception>(() => _service.Execute(input));
    }

    [Fact]
    public void Execute_ShouldThrow_WhenSortingOrderIsInvalid()
    {
        var input = "random\napple\nbanana";
        Assert.Throws<Exception>(() => _service.Execute(input));
    }
}