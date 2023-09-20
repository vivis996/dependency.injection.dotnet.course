using Autofac;

namespace Dependency.Injection;

public interface IVehicle
{
    void Go();
}

public class Truck : IVehicle
{
    private readonly IDriver _driver;

    public Truck(IDriver driver)
    {
        this._driver = driver;
    }

    public void Go()
    {
        this._driver.Drive();
    }
}

public interface IDriver
{
    void Drive();
}

public class CrazyDriver : IDriver
{
    public void Drive()
    {
        Console.WriteLine("Going too fast and crashing into a tree");
    }
}

public class SaneDriver : IDriver
{
    public void Drive()
    {
        Console.WriteLine("Driver safely to destination");
    }
}

public class TransportModule : Module
{
    public bool ObeySpeedLimit { get; set; }

    protected override void Load(ContainerBuilder builder)
    {
        if (this.ObeySpeedLimit)
            builder.RegisterType<SaneDriver>().As<IDriver>();
        else
            builder.RegisterType<CrazyDriver>().As<IDriver>();

        builder.RegisterType<Truck>().As<IVehicle>();
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule(new TransportModule() { ObeySpeedLimit = true });

        using (var c = builder.Build())
        {
            c.Resolve<IVehicle>().Go();
        }
    }
}