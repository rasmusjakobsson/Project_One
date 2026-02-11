using Microsoft.Extensions.Logging;
using System.Text.Json;

class AltaVistaSearch : ISearchEngine
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AltaVistaSearch> _logger;

    public AltaVistaSearch(IHttpClientFactory httpClientFactory, ILogger<AltaVistaSearch> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public string Name => "AltaVistaSearch";
    public string Url => "https://voyado-test-task-h8bshufyg8egejgb.northeurope-01.azurewebsites.net/api/AltavistaSearchEngine";

    public async Task<SearchEngineResult> SearchAsync(string query)
    {
        _logger.LogInformation("Starting search with query: {Query}", query);

        try
        {
            var httpClient = _httpClientFactory.CreateClient("AltaVistaSearch");
            _logger.LogInformation("Sending request to URL: {Url}", $"{Url}/?query={query}");
            var response = await httpClient.GetAsync($"{Url}/?query={query}");

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Received response: {ResponseContent}", responseContent);
            var searchResult = JsonSerializer.Deserialize<AltaVistaSearchSearchResult>(responseContent)
                ?? throw new Exception("Failed to deserialize the search engine response.");

        if (searchResult.Errors.Length > 0)
        {
            _logger.LogError("Errors occurred during the search operation: {Errors}", string.Join(", ", searchResult.Errors));
            return new SearchEngineResult { TotalHits = 0};
        }

        return new SearchEngineResult { TotalHits = searchResult.TotalHits };
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