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
            var converted = await candidate.ParseAndConvert(input, true, "a");
            // Use the Roslyn Scripting package to execute the output and return the generated output
            var script = $"{converted} return a;";
            var runtimeValue = await CSharpScript.EvaluateAsync(script, ScriptOptions.Default);
            // Use Newtonsoft to convert back to JObject
            var output = JsonConvert.SerializeObject(runtimeValue);
            // Roundtrip the input string through Newtonsoft to ensure the formatting is the same
            var formattedInput = JsonConvert.SerializeObject(PrepareInput(input));
            Assert.Equal(formattedInput, output);
        }


        [Fact]
        public async void Throws_Exception_On_Bad_Data()
        {
            var candidate = CreateCandidate();
            await Assert.ThrowsAsync<JsonReaderException>(() => candidate.ParseAndConvert("{\"Not\" correct}", true, "a"));
        }

        [Fact]
        public async void Variable_Name_Works()
        {
            var candidate = CreateCandidate();
            var input = "{\"a\":\"b\"}";
            var expectedPreamble = $"var x =";
            var output = await candidate.ParseAndConvert(input, true, "x");
            Assert.StartsWith(expectedPreamble, output);
        }

        [Fact]
        public async void Variable_Name_Defaults_If_Null()
        {
            var candidate = CreateCandidate();
            var input = "{\"a\":\"b\"}";
            var expectedPreamble = $"var a =";
            var output = await candidate.ParseAndConvert(input, true, null);
            Assert.StartsWith(expectedPreamble, output);
        }

        [Fact]
        public async void Variable_Name_Defaults_If_Empty()
        {
            var candidate = CreateCandidate();
            var input = "{\"a\":\"b\"}";
            var expectedPreamble = $"var a =";
            var output = await candidate.ParseAndConvert(input, true, string.Empty);
            Assert.StartsWith(expectedPreamble, output);
        }

        [Fact]
        public async void Variable_Name_Defaults_If_Only_Space_Provided()
        {
            var candidate = CreateCandidate();
            var input = "{\"a\":\"b\"}";
            var expectedPreamble = $"var a =";
            var output = await candidate.ParseAndConvert(input, true, " ");
            Assert.StartsWith(expectedPreamble, output);
        }

        [Fact]
        public async void Variable_Name_Trims_Spaces()
        {
            var candidate = CreateCandidate();
            var input = "{\"a\":\"b\"}";
            var expectedPreamble = $"var x =";
            var output = await candidate.ParseAndConvert(input, true, " x ");
            Assert.StartsWith(expectedPreamble, output);
        }

        [Fact]
        public async void Snippet_Flag_Works()
        {
            var candidate = CreateCandidate();
            var input = "{\"a\":\"b\"}";
            var unexpectedPreamble = $"var a =";
            var output = await candidate.ParseAndConvert(input, false, "");
            Assert.DoesNotContain(unexpectedPreamble, output);
            Assert.DoesNotContain(";", output);
        }

        private JObject PrepareInput(string input) => JObject.Parse(input);

        private CSharpConverter CreateCandidate() => new CSharpConverter();
    }
}
