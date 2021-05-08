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

        private JObject PrepareInput(string input) => JObject.Parse(input);

        private CSharpConverter CreateCandidate() => new CSharpConverter();
    }
}
