using Autofac;
using Autofac.Features.Indexed;

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
    private readonly ILog _log;
    private readonly int _id;

    public Engine(ILog log)
    {
        this._log = log;
        this._id = new Random().Next();
    }

    public Engine(ILog log, int id)
    {
        this._log = log;
        this._id = id;
    }

    public void Ahead(int power)
    {
        this._log.Write($"Engine [{this._id}] ahead {power}");
    }
}

public class Car
{
    private readonly Engine _engine;
    private readonly ILog _log;

    public Car(Engine engine)
    {
        this._engine = engine;
        this._log = new EmailLog();
    }

    public Car(Engine engine, ILog log)
    {
        this._engine = engine;
        this._log = log;
    }

    public void Go()
    {
        _engine.Ahead(100);
        _log.Write("Car going forward...");
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

    public Child()
    {
        Console.WriteLine("Child being created");
    }

    public void SetParent(Parent parent)
    {
        this.Parent = parent;
    }

    public override string ToString()
    {
        return "Hi there";
    }
}

public class BadChild : Child
{
    public override string ToString()
    {
        return "I hate you";
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
        builder.RegisterType<Parent>();
        builder.RegisterType<Child>()
               .OnActivating(a =>
               {
                   Console.WriteLine("Child activating");
                   //a.Instance.Parent = a.Context.Resolve<Parent>();
                   a.ReplaceInstance(new BadChild());
               })
               .OnActivated(a =>
               {
                   Console.WriteLine("Child activated");
               })
               .OnRelease(a =>
               {
                   Console.WriteLine("Child about to be removed");
               });

        // Not work
        //builder.RegisterType<ConsoleLog>()
        //       .As<ILog>()
        //       .OnActivating(a =>
        //       {
        //           a.ReplaceInstance(new SMSLog("+123"));
        //       });

        builder.RegisterType<ConsoleLog>().AsSelf();
        builder.Register<ILog>(c => c.Resolve<ConsoleLog>())
               .OnActivating(a => a.ReplaceInstance(new SMSLog("+123")));

        using (var scope = builder.Build().BeginLifetimeScope())
        {
            var child = scope.Resolve<Child>();
            var parent = child.Parent;
            Console.WriteLine(child);
            Console.WriteLine(parent);

            var log = scope.Resolve<ILog>();
            log.Write("Testing");
        }
    }
}