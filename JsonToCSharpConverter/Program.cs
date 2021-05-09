using Avalonia;
using Avalonia.ReactiveUI;
using JsonToCSharpConverter.Extensions;
using Autofac;
using JsonToCSharpConverter.Abstracts;
using JsonToCSharpConverter.ViewModels;

namespace JsonToCSharpConverter
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI()
                .AddAutofacContainer(b =>
                {
                    b.RegisterType<CSharpConverter>().As<ICSharpConverter>().SingleInstance();
                    b.RegisterType<MainWindowViewModel>();
                });
    }
}
