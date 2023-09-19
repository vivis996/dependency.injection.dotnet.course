﻿using Autofac;
using Autofac.Features.Metadata;

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
    private readonly Meta<ConsoleLog, Settings> _log;

    public Reporting(Meta<ConsoleLog, Settings> log)
    {
        this._log = log;
    }

    public void Report()
    {
        this._log.Value.Write("Starting report");

        if (this._log.Metadata.LogMode == "verbose")
        {
            this._log.Value.Write($"VERBOSE MODEL: Logger started on {DateTime.Now}");
        }
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        //builder.RegisterType<ConsoleLog>().WithMetadata("mode", "verbose");
        builder.RegisterType<ConsoleLog>()
               .WithMetadata<Settings>(c => c.For(x => x.LogMode, "verbose"));
        builder.RegisterType<Reporting>();
        using (var c = builder.Build())
        {
            c.Resolve<Reporting>().Report();
        }
    }
}