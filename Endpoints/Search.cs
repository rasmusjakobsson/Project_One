using Project_One;
using Project_One.Services;

namespace Project_One.Endpoints;

public class Search
{
    private readonly SearchHandler _searchHandler;

    public Search(SearchHandler searchHandler)
    {
        _searchHandler = searchHandler;
    }

    public void RegisterEndpoints(WebApplication app)
    {
        app.MapPost("/search", async (HttpContext context, ILogger<Search> logger, IEnumerable<ISearchEngine> searchEngines) =>
        {
            logger.LogInformation("Search endpoint: reading request body");
            using var reader = new StreamReader(context.Request.Body);
            var requestBody = await reader.ReadToEndAsync();
            logger.LogInformation("Search endpoint: calling SearchHandler for {Length} chars", requestBody?.Length ?? 0);

            var results = await _searchHandler.ExecuteAsync(requestBody, searchEngines);

            if (results is null)
            {
                logger.LogWarning("Received an empty search request.");
                return Results.BadRequest("Input cannot be empty.");
            }

            logger.LogInformation("result complete with {Count} results", results.Count());
            context.Response.ContentType = "application/json";
            return Results.Json(results);
        });
    }
}