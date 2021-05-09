using JsonToCSharpConverter.Abstracts;
using JsonToCSharpConverter.ViewModels;
using Moq;
using Xunit;

namespace JsonToCSharpConverter.Tests
{
    public class MainWindowViewModelTests
    {
        private Mock<ICSharpConverter> _mockCSharpConvert;

        public MainWindowViewModelTests()
        {
            _mockCSharpConvert = new Mock<ICSharpConverter>();
        }

        [Fact]
        public void Initial_Call_Made_To_CSharpConverter()
        {
            using (var candidate = CreateCandidate())
            {
                _mockCSharpConvert.Verify(x => x.ParseAndConvert(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once());
            }
        }

        [Fact]
        public void Changed_InputValue_Passed_To_CSharpConverter()
        {
            using (var candidate = CreateCandidate())
            {
                var input = "{\"a\":\"b\"}";
                candidate.InputValue = input;
                _mockCSharpConvert.Verify(x => x.ParseAndConvert(input, It.IsAny<bool>(), It.IsAny<string>()), Times.Once());
            }
        }

        [Fact]
        public void Changed_SnippetFlag_Passed_To_CSharpConverter()
        {
            using (var candidate = CreateCandidate())
            {
                candidate.GenerateFullSnippet = false;
                _mockCSharpConvert.Verify(x => x.ParseAndConvert(It.IsAny<string>(), false, It.IsAny<string>()), Times.Once());
            }
        }


        [Fact]
        public void Changed_VariableName_Passed_To_CSharpConverter()
        {
            using (var candidate = CreateCandidate())
            {
                candidate.VariableName = "b";
                _mockCSharpConvert.Verify(x => x.ParseAndConvert(It.IsAny<string>(), It.IsAny<bool>(), "b"), Times.Once());
            }
        }

        [Fact]
        public void Change_Subscription_Disposed_On_Disposal()
        {
            var candidate = CreateCandidate();
            // Dispose of Candidate and confirm that change subscription is no longer active
            candidate.Dispose();
            candidate.GenerateFullSnippet = false;
            _mockCSharpConvert.Verify(x => x.ParseAndConvert(It.IsAny<string>(), false, It.IsAny<string>()), Times.Never());
        }

        private MainWindowViewModel CreateCandidate() => new MainWindowViewModel(_mockCSharpConvert.Object);
    }
}