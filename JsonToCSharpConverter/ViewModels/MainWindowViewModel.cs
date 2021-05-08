using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace JsonToCSharpConverter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly IDisposable _inputSubscription;
        private string _inputValue = "{\"Paste\": \"JSON Here\"}";
        private string _outputValue = "Result will appear here";
        private bool disposedValue;

        public MainWindowViewModel()
        {
            var converter = new CSharpConverter();
            _inputSubscription
                = this.WhenAnyValue(x => x.InputValue)
                    .Subscribe(async x =>
                    {
                        try
                        {
                            OutputValue = await converter.ParseAndConvert(x);
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
