using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System;

namespace JsonToCSharpConverter.Extensions
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder AddAutofacContainer(this AppBuilder appBuilder,
            Action<ContainerBuilder, Application> containerBuilderSetupAction)
            => appBuilder.AfterSetup(x =>
                {
                    // Create ContainerBuilder and pass to setup action
                    var builder = new Autofac.ContainerBuilder();
                    containerBuilderSetupAction(builder, Application.Current);
                    var container = builder.Build();
                    // Store in the AvaloniaLocator
                    AvaloniaLocator.CurrentMutable.StoreAutofacContainer(container);

                    // Local method to be called on shutdown to dispose of container
                    void Shutdown(object sender, ControlledApplicationLifetimeExitEventArgs e)
                    {
                        container.Dispose();
                        switch (sender)
                        {
                            case IClassicDesktopStyleApplicationLifetime desktop:
                                desktop.Exit -= Shutdown;
                                break;

                            case IControlledApplicationLifetime controlled:
                                controlled.Exit -= Shutdown;
                                break;
                        }
                    };

                    // Attach to Exit event
                    switch (Application.Current.ApplicationLifetime)
                    {
                        case IClassicDesktopStyleApplicationLifetime desktop:
                            desktop.Exit += Shutdown;
                            break;

                        case IControlledApplicationLifetime controlled:
                            controlled.Exit += Shutdown;
                            break;
                    }
                });
    }
}