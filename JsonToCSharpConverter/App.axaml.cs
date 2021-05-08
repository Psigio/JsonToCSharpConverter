using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JsonToCSharpConverter.ViewModels;
using JsonToCSharpConverter.Views;

namespace JsonToCSharpConverter
{
    public class App : Application
    {
        private MainWindowViewModel _context;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _context = new MainWindowViewModel();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = _context,
                };
                desktop.Exit += HandleExit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void HandleExit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            _context?.Dispose();
            var source = sender as IClassicDesktopStyleApplicationLifetime;
            if (source != null)
            {
                source.Exit -= HandleExit;
            }
        }
    }
}