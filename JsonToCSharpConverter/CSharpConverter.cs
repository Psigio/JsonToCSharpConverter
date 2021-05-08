using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Newtonsoft.Json.Linq;

namespace JsonToCSharpConverter
{
    public class CSharpConverter
    {
        public Task<string> ParseAndConvert(string inputJson)
            => ParseAndConvert(inputJson, true, "a");

        public Task<string> ParseAndConvert(string inputJson, bool generateFullSnippet, string variableName)
            => Task.Factory.StartNew<string>(() =>
            {
                // To get the formatting logic to work we need to create a full snippet first and adjust afterwards
                var notNullVariableName = string.IsNullOrWhiteSpace(variableName)
                    ? "a"
                    : variableName.Trim();
                var prefix = $"var {notNullVariableName} = ";
                var unformatted = $"{prefix}{Convert(JObject.Parse(inputJson))};";
                var formatted = Format(unformatted);
                return generateFullSnippet
                    ? formatted
                    : formatted.Substring(prefix.Length).TrimEnd(';');
            });

        private string Format(string inputCode)
        {
            var tree = CSharpSyntaxTree.ParseText(inputCode);
            using (var adhocWorkspace = new AdhocWorkspace())
            {
                var solutionInfo = Microsoft.CodeAnalysis.SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Default);
                adhocWorkspace.AddSolution(solutionInfo);
                var root = tree.GetCompilationUnitRoot().NormalizeWhitespace(" ", true);
                var formatted = Formatter.Format(root, adhocWorkspace);
                return formatted.ToString();
            }
        }

        internal string Convert(JToken input)
        {
            // Build string
            var sb = new StringBuilder();
            switch (input)
            {
                case JArray array:
                    {
                        sb.Append("new [] {");
                        var values = array
                            .Where(x => x != null)
                            .Select(v => Convert(v));
                        sb.AppendJoin(",", values);
                        sb.Append("}");
                    }
                    break;
                case JObject jObject:
                    {
                        sb.Append("new {");
                        var values = jObject
                            .Properties()
                            .Where(x => x != null)
                            .Select(v => Convert(v));
                        sb.AppendJoin(",", values);
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
