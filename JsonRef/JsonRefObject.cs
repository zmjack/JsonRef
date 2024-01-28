using System;
using System.Text.Json.Serialization;

namespace JsonRef;

public struct JsonRefObject
{
    [JsonPropertyName("$ref")]
    public Guid Ref { get; set; }
}
