using Autofac;
using Avalonia;

namespace JsonToCSharpConverter.Extensions
{
    public static class AvaloniaLocatorExtensions
    {
        public static void StoreAutofacContainer(this AvaloniaLocator avaloniaLocator, IContainer container)
            => avaloniaLocator.BindToSelf<IContainer>(container);
    }
}