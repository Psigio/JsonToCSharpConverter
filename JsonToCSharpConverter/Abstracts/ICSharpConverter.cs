using System.Threading.Tasks;

namespace JsonToCSharpConverter.Abstracts
{
    public interface ICSharpConverter
    {
        Task<string> ParseAndConvert(string inputJson, bool generateFullSnippet, string variableName);
    }
}