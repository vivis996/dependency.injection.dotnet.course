using System.Collections.Generic;
using Autofac;

namespace Dependency.Injection;

public interface IResource
{
}

public class SingletonResource : IResource
{
}

public class InstancePerDependencyResource : IResource, IDisposable
{
    public InstancePerDependencyResource()
    {
        Console.WriteLine("Instance per dep created");
    }

    public void Dispose()
    {
        Console.WriteLine("Instance per dep destroyed");
    }
}

public class ResourceManager
{
    public IEnumerable<IResource> Resources { get; set; }

    public ResourceManager(IEnumerable<IResource> resources)
    {
        Resources = resources;
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<ResourceManager>().SingleInstance();
        builder.RegisterType<SingletonResource>()
               .As<IResource>()
               .SingleInstance();
        builder.RegisterType<InstancePerDependencyResource>()
               .As<IResource>();

        using (var container = builder.Build())
        using(var scope = container.BeginLifetimeScope())
        {
            scope.Resolve<ResourceManager>();
        }
    }
}