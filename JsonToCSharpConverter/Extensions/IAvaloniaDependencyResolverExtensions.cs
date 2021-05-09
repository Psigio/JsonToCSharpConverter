using Autofac;
using Avalonia;

namespace JsonToCSharpConverter.Extensions
{
    public static class IAvaloniaDependencyResolverExtensions
    {
        public static IContainer GetAutofacContainer(this IAvaloniaDependencyResolver dependencyResolver)
            => dependencyResolver.GetService<IContainer>();
    }
}