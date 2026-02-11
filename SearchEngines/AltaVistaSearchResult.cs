using System.Text.Json.Serialization;

public class AltaVistaSearchSearchResult
{
    [JsonPropertyName("totalHits")]
    public long TotalHits { get; set; }

    [JsonPropertyName("query")]
    public string Query { get; set; } 

    [JsonPropertyName("searchHits")]
    public string[] SearchHits { get; set; }


    [JsonPropertyName("errors")]
    public string[] Errors { get; set; } 

}



