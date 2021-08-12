using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;

namespace Dependency.Injection
{
    public interface ILog
    {
        void Write(string message);
    }

    public interface IConsole
    {

    }

    public class ConsoleLog : ILog, IConsole
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class EmailLog : ILog
    {
        private const string AdminEmail = "admin@mail.com";

        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {AdminEmail} : {message}.");
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
    }

    public class Engine
    {
        private ILog log;
        private int id;

        public Engine(ILog log)
        {
            this.log = log;
            this.id = new Random().Next();
        }

        public Engine(ILog log, int id)
        {
            this.log = log;
            this.id = id;
        }

        public void Ahead(int power)
        {
            this.log.Write($"Engine [{id}] ahead {power}.");
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
            this.engine.Ahead(100);
            this.log.Write("Car going foward...");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // Name parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter("phoneNumber", "+123456789");

            // Type parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter(new TypedParameter(typeof(string), "+12456789"));

            // Resolved parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter(
            //        new ResolvedParameter(
            //            // Predicate
            //            (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
            //            // Value accessor
            //            (pi, ctx) => "+12456789"
            //       )
            //    );

            var randon = new Random();
            builder.Register((c, p) => new SMSLog(p.Named<string>("phoneNumber"))).As<ILog>();

            Console.WriteLine("About to build container...");
            var continer = builder.Build();
            var log = continer.Resolve<ILog>(new NamedParameter("phoneNumber", randon.Next().ToString()));
            log.Write("Test message.");
        }
    }
}
