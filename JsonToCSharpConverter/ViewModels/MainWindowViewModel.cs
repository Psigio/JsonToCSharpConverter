using System;
using JsonToCSharpConverter.Abstracts;
using ReactiveUI;

namespace JsonToCSharpConverter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly IDisposable _inputSubscription;
        private string _inputValue = "{\"Paste\": \"JSON Here\"}";
        private string _outputValue = "Result will appear here";
        private bool disposedValue;
        private bool _generateFullSnippet = true;
        private string _variableName = "a";

        public MainWindowViewModel(ICSharpConverter cSharpConverter)
        {
            _inputSubscription
                = this.WhenAnyValue(x => x.InputValue, x => x.GenerateFullSnippet, x => x.VariableName)
                    .Subscribe(async anon =>
                    {
                        try
                        {
                            var (inputValue, generateFullSnippet, variableName) = anon;
                            OutputValue = await cSharpConverter.ParseAndConvert(inputValue, generateFullSnippet, variableName);
                        }
                        catch (Exception ex)
                        {
                            OutputValue = $"Not valid Json: {ex.Message}";
                        }
                    });
        }

        public string InputValue
        {
            get => _inputValue;
            set => this.RaiseAndSetIfChanged(ref _inputValue, value);
        }

        public string OutputValue
        {
            get => _outputValue;
            set => this.RaiseAndSetIfChanged(ref _outputValue, value);
        }

        public bool GenerateFullSnippet
        {
            get => _generateFullSnippet;
            set => this.RaiseAndSetIfChanged(ref _generateFullSnippet, value);
        }

        public string VariableName
        {
            get => _variableName;
            set => this.RaiseAndSetIfChanged(ref _variableName, value);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _inputSubscription.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
