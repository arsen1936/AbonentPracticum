namespace WebApp.Api.Tests.Services;

public class UuidGeneratorServiceTests
{
    private readonly UuidGeneratorService _service = new();

    [Fact]
    public void Execute_ShouldReturnValidUUID()
    {
        var result = _service.Execute("1");
        Assert.True(Guid.TryParse(result, out _));
    }

    [Fact]
    public void Execute_ShouldReturnRequestedCount()
    {
        var result = _service.Execute("3");
        Assert.Equal(3, result.Split("\n").Length);
    }

    [Fact]
    public void Execute_ShouldReturnUniqueUUID()
    {
        var result = _service.Execute("10");
        Assert.Equal(10, result.Split("\n").Distinct().Count());
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("-1")]
    [InlineData("1.5")]
    [InlineData("0")]
    public void Execute_InvalidInput_ShouldThrow(string input)
    {
        Assert.Throws<ArgumentException>(() => _service.Execute(input));
    }
}