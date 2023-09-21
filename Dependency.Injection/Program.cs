using Autofac;

namespace Dependency.Injection;

internal class Program
{
    static void Main(string[] args)
    {
        var containerBuilder = new ContainerBuilder();

        using (var container = containerBuilder.Build())
        {
        }
    }
}