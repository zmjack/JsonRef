using Microsoft.ClearScript.V8;
using NStandard;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JsonRef.Test;

public partial class NormalTest
{
    [GeneratedRegex(@"\w{8}-\w{4}-\w{4}-\w{4}-\w{12}")]
    private static partial Regex GuidRegex();

    [Fact]
    public void Test1()
    {
        var head = new Head
        {
            Name = "Root",
        };

        Part[] parts =
        [
            new Part(new DateOnly(2020, 1, 1), 100) { HeadLink = head },
            new Part(new DateOnly(2020, 2, 1), 200) { HeadLink = head },
        ];
        head.Parts = parts;

        var json = JsonSerializer.Serialize(JsonRefRoot.Parse(head), new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        var excepted = """
{
  "$refs": {
    "<id 1>": {
      "$type": "JsonRef.Test.Head",
      "$": {
        "name": "Root",
        "parts": [
          {
            "$ref": "<id 2>"
          },
          {
            "$ref": "<id 3>"
          }
        ]
      }
    },
    "<id 2>": {
      "$type": "JsonRef.Test.Part",
      "$": {
        "saleDate": "2020-01-01",
        "count": 100,
        "headLink": {
          "$ref": "<id 1>"
        }
      }
    },
    "<id 3>": {
      "$type": "JsonRef.Test.Part",
      "$": {
        "saleDate": "2020-02-01",
        "count": 200,
        "headLink": {
          "$ref": "<id 1>"
        }
      }
    }
  },
  "$": {
    "$ref": "<id 1>"
  }
}
""";

        var regex = GuidRegex();
        var matches = regex.Matches(json).Select(x => x.Value).Distinct().ToArray();
        foreach (var pair in matches.Pairs())
        {
            var value = pair.Value;
            json = json.Replace(value, $"<id {pair.Index + 1}>");
        }

        Assert.Equal(excepted, json);

        using var engine = new V8ScriptEngine();
        var js = File.ReadAllText("NormalTest.cs.js");
        engine.Execute(js);
        string result = engine.ExecuteCommand($"eval_jsonref({json}).parts[0].headLink.name");
        Assert.Equal("Root", result);
    }

}
