using Autofac;
using Avalonia;
using System;

namespace JsonToCSharpConverter.Extensions
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder AddAutofacContainer(this AppBuilder appBuilder, Action<ContainerBuilder> containerBuilderFunc)
        {
            appBuilder.AfterSetup(x =>
            {
                var b = new Autofac.ContainerBuilder();
                containerBuilderFunc(b);
                var container = b.Build();
                AvaloniaLocator.CurrentMutable.BindToSelf<IContainer>(container);
                // TODO - detect shutdown and dispose         
                // var lifetime = Application.Current.ApplicationLifetime;
            });
            return appBuilder;
        }
    }
}