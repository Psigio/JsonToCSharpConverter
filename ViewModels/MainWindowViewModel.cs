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
            _inputSubscription
                = this.WhenAnyValue(x => x.InputValue)
                    .Subscribe(x => ParseAndConvert(x));
        }

        private void ParseAndConvert(string inputJson)
        {
            try
            {
                var jObject = JObject.Parse(inputJson);
                OutputValue = Convert(jObject) + ";";
            }
            catch (Exception ex)
            {
                OutputValue = $"Not valid Json: {ex.Message}";
            }
        }

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
                            case JTokenType.Boolean:
                                sb.Append(rawValue);
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
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
