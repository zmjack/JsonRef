using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonRef;

public static class JsonRefRoot
{
    public static JsonRefRoot<T> Parse<T>(T obj) where T : class => new(obj);
}

public class JsonRefRoot<T> where T : class
{
    [JsonPropertyName("$refs")]
    public Dictionary<Guid, JsonRefElement> Refs { get; } = new();

    [JsonPropertyName("$")]
    public JsonRefObject Value { get; set; }

    private Dictionary<object, Guid> _pointers = new();

    public JsonRefRoot(T obj)
    {
        var node = new JsonRefNode(_pointers, Refs, typeof(T), obj);
        Value = new JsonRefObject
        {
            Ref = node.Ref
        };
    }
}
