using Autofac;
using Autofac.Features.Indexed;
using Autofac.Features.Metadata;
using static System.Formats.Asn1.AsnWriter;

namespace Dependency.Injection;

public interface ILog : IDisposable
{
    void Write(string message);
}

public interface IConsole
{

}

public class ConsoleLog : ILog, IConsole
{
    public ConsoleLog()
    {
        Console.WriteLine($"Console log created at {DateTime.Now.Ticks}.");
    }

    public void Write(string message)
    {
        Console.WriteLine(message);
    }

    public void Dispose()
    {
        Console.WriteLine($"Console logger no longer required.");
    }
}

public class EmailLog : ILog
{
    private const string AdminEmail = "admin@mail.com";

    public void Write(string message)
    {
        Console.WriteLine($"Email sent to {AdminEmail} : {message}.");
    }

    public void Dispose()
    {
        Console.WriteLine($"Email logger no longer required.");
    }
}

public class SMSLog : ILog
{
    private readonly string phoneNumber;

    public SMSLog(string phoneNumber)
    {
        this.phoneNumber = phoneNumber;
    }

    public void Write(string message)
    {
        Console.WriteLine($"SMS to {phoneNumber} : {message}");
    }

    public void Dispose()
    {
        Console.WriteLine($"SMS logger no longer required.");
    }
}

public class Settings
{
    public string LogMode { get; set; }

    //
}

public class Reporting
{
    private readonly IIndex<string, ILog> _logs;

    public Reporting(IIndex<string, ILog> logs)
    {
        this._logs = logs;
    }

    public void Report()
    {
        this._logs["sms"].Write("Starting report output");
    }
}

public class Engine
{
    private ILog log;
    private int id;

    public Engine(ILog log)
    {
        this.log = log;
        id = new Random().Next();
    }

    public Engine(ILog log, int id)
    {
        this.log = log;
        this.id = id;
    }

    public void Ahead(int power)
    {
        log.Write($"Engine [{id}] ahead {power}");
    }
}

public class Car
{
    private Engine engine;
    private ILog log;

    public Car(Engine engine)
    {
        this.engine = engine;
        this.log = new EmailLog();
    }

    public Car(Engine engine, ILog log)
    {
        this.engine = engine;
        this.log = log;
    }

    public void Go()
    {
        engine.Ahead(100);
        log.Write("Car going forward...");
    }
}

public class Parent
{
    public override string ToString()
    {
        return "I am your father";
    }
}

public class Child
{
    public string Name { get; set; }
    public Parent Parent { get; set; }

    public void SetParent(Parent parent)
    {
        Parent = parent;
    }
}

public class ParentChildModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Parent>();
        builder.Register(c => new Child() { Parent = c.Resolve<Parent>() });
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<ConsoleLog>()
               // Just create one instance using a tag
               .InstancePerMatchingLifetimeScope("foo");

        var container = builder.Build();

        using (var scope1 = container.BeginLifetimeScope("foo"))
        {
            for (var i = 0; i < 3; i++)
            {
                scope1.Resolve<ConsoleLog>();
            }

            using (var scope2 = scope1.BeginLifetimeScope())
            {
                for (var i = 0; i < 3; i++)
                {
                    scope2.Resolve<ConsoleLog>();
                }
            }
        }

        // It will crash, because it doesn't contain the same tag 'foo'
        // this will also happen if you use another tag
        using (var scope3 = container.BeginLifetimeScope())
        {
            for (var i = 0; i < 3; i++)
            {
                scope3.Resolve<ConsoleLog>();
            }
        }
    }
}