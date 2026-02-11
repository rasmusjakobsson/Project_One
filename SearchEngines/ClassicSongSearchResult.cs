using System.Text.Json.Serialization;

public class ClassicSongSearchResult
{

        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("totalSearchHits")]
        public long TotalSearchHits { get; set; }

        [JsonPropertyName("findHits")]
        public string[] FindHits { get; set; }
        
        [JsonPropertyName("errors")]
        public string[] Errors { get; set; }
}