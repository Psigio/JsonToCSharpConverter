using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JsonToCSharpConverter
{
    public class Converter
    {
        public string ParseAndConvert(string inputJson)
            => Convert(JObject.Parse(inputJson)) + ";";

        private string Convert(JToken input)
        {
            // Build string
            var sb = new StringBuilder();
            switch (input)
            {
                case JArray array:
                    {
                        sb.AppendLine("new [] {");
                        var values = array
                            .Where(x => x != null)
                            .Select(v => Convert(v));
                        sb.AppendJoin($",{Environment.NewLine}", values);
                        sb.Append("}");
                    }
                    break;
                case JObject jObject:
                    {
                        sb.AppendLine("new {");
                        var values = jObject
                            .Properties()
                            .Where(x => x != null)
                            .Select(v => Convert(v));
                        sb.AppendJoin($",{Environment.NewLine}", values);
                        sb.Append("}");
                    }
                    break;
                case JProperty jProperty:
                    {
                        var preamble = jProperty.Parent?.Type != JTokenType.Array
                                ? $"{jProperty.Name} = "
                                : "";
                        sb.Append(preamble);
                        sb.Append(Convert(jProperty.Value));
                        break;
                    }
                case JValue jValue:
                    {
                        var rawValue = jValue.Value?.ToString();
                        switch (jValue.Type)
                        {
                            case JTokenType.Object:
                            case JTokenType.Array:
                                sb.Append(Convert(jValue));
                                break;
                            case JTokenType.Float:
                            case JTokenType.Integer:
                                sb.Append(rawValue);
                                break;
                            case JTokenType.Boolean:
                                sb.Append(rawValue.ToLowerInvariant());
                                break;
                            default:
                                sb.Append($"\"{jValue.Value?.ToString()}\"");
                                break;
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException($"JContainer type {input.GetType().FullName} was not expected");
            }
            return sb.ToString();
        }

    }
}
