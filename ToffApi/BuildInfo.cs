using System.Text.Json.Serialization;

namespace ToffApi;

public class BuildInfo
{
    [JsonPropertyName("buildName")]
    public string BuildName { get; set; }
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
    [JsonPropertyName("branch")]
    public string Branch { get; set; }
    [JsonPropertyName("buildDate")]
    public string BuildDate { get; set; }
}
