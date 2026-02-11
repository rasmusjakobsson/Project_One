using Moq;
using Project_One;
using Project_One.Services;
using Xunit;

namespace Project_One.Tests;

public class SearchHandlerTests
{
    private readonly SearchHandler _handler = new();

    [Fact]
    public async Task ExecuteAsync_WhenRequestBodyIsNull_ReturnEmptyList()
    {
        var engines = Array.Empty<ISearchEngine>();
        var result = await _handler.ExecuteAsync(null, engines);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRequestBodyIsEmpty_ReturnsEmptyList()
    {
        var engines = Array.Empty<ISearchEngine>();
        var result = await _handler.ExecuteAsync("", engines);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRequestBodyIsWhitespace_ReturnsNull()
    {
        var engines = Array.Empty<ISearchEngine>();
        var result = await _handler.ExecuteAsync("   \t\n  ", engines);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_WithOneWordAndOneEngine_ReturnsSumOfHits()
    {
        var engine = new Mock<ISearchEngine>();
        engine.Setup(e => e.Name).Returns("TestEngine");
        engine.Setup(e => e.SearchAsync("hello"))
            .ReturnsAsync(new SearchEngineResult { TotalHits = 42 });

        var result = await _handler.ExecuteAsync("hello", new[] { engine.Object });

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(42L, result.First().TotalHits);
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleWordsAndOneEngine_AggregatesHits()
    {
        var engine = new Mock<ISearchEngine>();
        engine.Setup(e => e.Name).Returns("TestEngine");
        engine.Setup(e => e.SearchAsync("hello")).ReturnsAsync(new SearchEngineResult { TotalHits = 10 });
        engine.Setup(e => e.SearchAsync("world")).ReturnsAsync(new SearchEngineResult { TotalHits = 20 });

        var result = await _handler.ExecuteAsync("hello world", new[] { engine.Object });

        Assert.NotNull(result);
        Assert.Equal(30L, result.First().TotalHits);
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleEngines_ReturnsPerEngineTotals()
    {
        var engine1 = new Mock<ISearchEngine>();
        engine1.Setup(e => e.Name).Returns("EngineA");
        engine1.Setup(e => e.SearchAsync(It.IsAny<string>()))
            .ReturnsAsync(new SearchEngineResult { TotalHits = 5, Engine = "EngineA" });

        var engine2 = new Mock<ISearchEngine>();
        engine2.Setup(e => e.Name).Returns("EngineB");
        engine2.Setup(e => e.SearchAsync(It.IsAny<string>()))
            .ReturnsAsync(new SearchEngineResult { TotalHits = 3 });

        var result = await _handler.ExecuteAsync("word", [engine1.Object, engine2.Object]);

        Assert.Equal(2, result.ToList().Count);
        Assert.Equal(5L, result[0].TotalHits);
        Assert.Equal(3L, result[1].TotalHits);
    }

    [Fact]
    public async Task ExecuteAsync_SplitsWordsBySpace_AndIgnoresExtraSpaces()
    {
        var engine = new Mock<ISearchEngine>();
        engine.Setup(e => e.Name).Returns("E");
        engine.Setup(e => e.SearchAsync("a")).ReturnsAsync(new SearchEngineResult { TotalHits = 1 });
        engine.Setup(e => e.SearchAsync("b")).ReturnsAsync(new SearchEngineResult { TotalHits = 2 });

        var result = await _handler.ExecuteAsync("  a   b  ", [engine.Object]);

        Assert.NotNull(result);
        Assert.Equal(3L, result.First().TotalHits);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSearchEngineReturnsErrors_AggregatesErrors()
    {
        var engine = new Mock<ISearchEngine>();
        engine.Setup(e => e.Name).Returns("E");
        engine.Setup(e => e.SearchAsync("a")).ReturnsAsync(new SearchEngineResult { TotalHits = 1, Errors = new[] { "Error1" } });
        engine.Setup(e => e.SearchAsync("b")).ReturnsAsync(new SearchEngineResult { TotalHits = 2, Errors = new[] { "Error2" } });

        var result = await _handler.ExecuteAsync("a b", [engine.Object]);

        Assert.NotNull(result);
        Assert.Equal(3L, result.First().TotalHits);
        Assert.Equal(2, result.First().Errors.Length);
        Assert.Contains("Error1", result.First().Errors);
        Assert.Contains("Error2", result.First().Errors);
    }
}
