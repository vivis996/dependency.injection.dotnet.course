using Autofac;
using Autofac.Features.Indexed;
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

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
    }
}