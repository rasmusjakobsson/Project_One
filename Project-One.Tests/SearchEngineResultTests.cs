using System.Text.Json;
using Project_One;
using Xunit;

namespace Project_One.Tests;

public class SearchEngineResultTests
{
    [Fact]
    public void SearchEngineResult_TotalHits_CanBeSetAndRead()
    {
        var result = new SearchEngineResult { TotalHits = 12345 };
        Assert.Equal(12345, result.TotalHits);
    }

    [Fact]
    public void AltaVistaSearchSearchResult_DeserializesFromJson()
    {
        var json = """{"totalHits":99,"query":"test","searchHits":[],"errors":[]}""";
        var result = JsonSerializer.Deserialize<AltaVistaSearchSearchResult>(json);
        Assert.NotNull(result);
        Assert.Equal(99, result.TotalHits);
        Assert.Equal("test", result.Query);
    }

    [Fact]
    public void ClassicSongSearchResult_DeserializesFromJson()
    {
        var json = """{"query":"test","totalSearchHits":42,"findHits":[],"errors":[]}""";
        var result = JsonSerializer.Deserialize<ClassicSongSearchResult>(json);
        Assert.NotNull(result);
        Assert.Equal(42, result.TotalSearchHits);
        Assert.Equal("test", result.Query);
    }
}
