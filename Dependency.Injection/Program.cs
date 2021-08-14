using System;
using Autofac;

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

    public class Parent
    {
        public override string ToString()
        {
            return "I'm your father";
        }
    }

    public class Child
    {
        public string Name { get; set; }
        public Parent Parent { get; set; }

        public void SetParent(Parent parent)
        {
            this.Parent = parent;
        }
    }

    public class ParentChildModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Parent>();
            builder.Register(c => new Child { Parent = c.Resolve<Parent>() });
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(Program).Assembly);
            builder.RegisterAssemblyModules<ParentChildModule>(typeof(Program).Assembly);

            var continer = builder.Build();
            Console.WriteLine(continer.Resolve<Child>().Parent);
        }
    }
}