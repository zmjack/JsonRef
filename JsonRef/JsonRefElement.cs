using System.Text.Json.Serialization;

namespace JsonRef;

public class JsonRefElement
{
    [JsonPropertyName("$type")]
    public string Type { get; set; }

    [JsonPropertyName("$")]
    public JsonRefNode Value { get; set; }
}
