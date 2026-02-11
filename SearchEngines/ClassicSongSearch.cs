using System.Text;
using System.Text.Json;

public class ClassicSongSearch : ISearchEngine
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ClassicSongSearch> _logger;

    public ClassicSongSearch(IHttpClientFactory httpClientFactory, ILogger<ClassicSongSearch> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public string Name => "ClassicSongSearch";
    public string Url => "https://voyado-test-task-h8bshufyg8egejgb.northeurope-01.azurewebsites.net/api/ClassicSongSearchEngine";

    public async Task<SearchEngineResult> SearchAsync(string query)
    {
        try
        {
            _logger.LogInformation("Starting search with query: {Query}", query);
            var httpClient = _httpClientFactory.CreateClient("ClassicSongSearch");
            _logger.LogInformation("Sending request to URL: {Url}", $"{Url}/?query={query}");
            var postBody = new { query };
            var jsonContent = new StringContent(JsonSerializer.Serialize(postBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(Url, jsonContent);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Received response: {ResponseContent}", responseContent);
            var searchResult = JsonSerializer.Deserialize<ClassicSongSearchResult>(responseContent)
                ?? throw new Exception("Failed to deserialize the search engine response.");

            if (searchResult.Errors.Length > 0)
            {
                _logger.LogError("Errors occurred during the search operation: {Errors}", string.Join(", ", searchResult.Errors));
                return new SearchEngineResult { Errors = searchResult.Errors };
            }

            return new SearchEngineResult { TotalHits = searchResult.TotalSearchHits };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the search operation.");
            if (ex is HttpRequestException httpEx)
            {
                _logger.LogError(ex, "HTTP Error: {httpEx.Message}", httpEx.Message);
            }
            return new SearchEngineResult { Errors = [ex.Message]};
        }
    }
}

