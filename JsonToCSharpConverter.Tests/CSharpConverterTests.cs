using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JsonToCSharpConverter.Tests
{
    public class ConverterTests
    {
        [Theory]
        [InlineData("{\"a\":\"b\"}", "new {a = \"b\"}")]
        [InlineData("{\"a\":5}", "new {a = 5}")]
        [InlineData("{\"a\":true}", "new {a = true}")]
        [InlineData("{\"a\":[\"b\", \"c\"]}", "new {a = new [] {\"b\",\"c\"}}")]
        [InlineData("{\"a\":[5, 6]}", "new {a = new [] {5,6}}")]
        [InlineData("{\"a\":[true, false]}", "new {a = new [] {true,false}}")]
        [InlineData("{\"inner\": { \"a\":\"b\", \"b\": 5, \"c\": true, \"d\":[true, false]}}", "new {inner = new {a = \"b\",b = 5,c = true,d = new [] {true,false}}}")]
        public void Convert_Basic_Json_Works(string input, string expected)
        {
            var candidate = CreateCandidate();
            var output = candidate.Convert(PrepareInput(input));
            Assert.Equal(expected, output);
        }

        [Theory]
        [InlineData("{\"a\":\"b\"}")]
        [InlineData("{\"a\":5}")]
        [InlineData("{\"a\":true}")]
        [InlineData("{\"a\":[\"b\", \"c\"]}")]
        [InlineData("{\"a\":[5, 6]}")]
        [InlineData("{\"a\":[true, false]}")]
        [InlineData("{\"inner\": { \"a\":\"b\", \"b\": 5, \"c\": true, \"d\":[true, false]}}")]
        public async void Roundtrip_Works(string input)
        {
            var candidate = CreateCandidate();
            var converted = await candidate.ParseAndConvert(input);
            // Use the Roslyn Scripting package to execute the output and return the generated output
            var script = $"{converted} return a;";
            var runtimeValue = await CSharpScript.EvaluateAsync(script, ScriptOptions.Default);
            // Use Newtonsoft to convert back to JObject
            var output = JsonConvert.SerializeObject(runtimeValue);
            // Roundtrip the input string through Newtonsoft to ensure the formatting is the same
            var formattedInput = JsonConvert.SerializeObject(PrepareInput(input));
            Assert.Equal(formattedInput, output);
        }

        private JObject PrepareInput(string input) => JObject.Parse(input);

        private CSharpConverter CreateCandidate() => new CSharpConverter();
    }
}
