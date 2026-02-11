public interface ISearchEngine
{
    string Name { get; }

    string Url { get; }

    Task<SearchEngineResult> SearchAsync(string query);
}



public class SearchEngineResult 
{
    public long TotalHits { get; set; }
    public string[] Errors { get; set; } = [];

    public string Engine { get; set; } = string.Empty;
}