using System;
using System.Reactive;
using System.Windows.Input;
using Avalonia.Input.Platform;
using Avalonia.Metadata;
using JsonToCSharpConverter.Abstracts;
using ReactiveUI;

namespace JsonToCSharpConverter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly IDisposable _inputSubscription;
        private readonly IClipboard _clipboard;
        private string _inputValue = "{\"Paste\": \"JSON Here\"}";
        private string _outputValue = "Result will appear here";
        private bool disposedValue;
        private bool _generateFullSnippet = true;
        private string _variableName = "a";
        private bool _isCopyEnabled = false;

        public MainWindowViewModel(ICSharpConverter cSharpConverter, IClipboard clipboard)
        {
            _clipboard = clipboard;
            _inputSubscription
                = this.WhenAnyValue(x => x.InputValue, x => x.GenerateFullSnippet, x => x.VariableName)
                    .Subscribe(async anon =>
                    {
                        try
                        {
                            var (inputValue, generateFullSnippet, variableName) = anon;
                            OutputValue = await cSharpConverter.ParseAndConvert(inputValue, generateFullSnippet, variableName);
                            IsCopyEnabled = true;
                        }
                        catch (Exception ex)
                        {
                            OutputValue = $"Not valid Json: {ex.Message}";
                            IsCopyEnabled = false;
                        }
                    });

            CopyCommand = ReactiveCommand.Create(DoCopy, this.WhenAnyValue(x => x.IsCopyEnabled));
        }

        private async void DoCopy()
        {
            await _clipboard.SetTextAsync(OutputValue);
        }

        public bool IsCopyEnabled
        {
            get => _isCopyEnabled;
            set
            {
                if (value == _isCopyEnabled) return;
                this.RaiseAndSetIfChanged(ref _isCopyEnabled, value);
            }
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

        public ReactiveCommand<Unit, Unit> CopyCommand { get; }

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
