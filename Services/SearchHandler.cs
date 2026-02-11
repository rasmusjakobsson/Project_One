namespace Project_One.Services;

public class SearchHandler
{
    public async Task<List<SearchEngineResult>> ExecuteAsync(string? requestBody, IEnumerable<ISearchEngine> searchEngines)
    {
        if (string.IsNullOrWhiteSpace(requestBody))
        {
            return [];
        }

        var words = requestBody.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var results = new List<SearchEngineResult>();

        foreach (var engine in searchEngines)
        {
            var searchTasks = words.Select(engine.SearchAsync);
                
            var completedTask = await Task.WhenAll(searchTasks);
            var engineResults = completedTask.Select(r => r.TotalHits).Sum();
            var errors = completedTask.SelectMany(r => r.Errors).ToArray();
            results.Add(new SearchEngineResult { Engine = engine.Name, TotalHits = engineResults, Errors = errors });
        }

        return results;
    }
}
