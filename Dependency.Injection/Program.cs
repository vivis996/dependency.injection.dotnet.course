using System;
using System.Collections.Generic;
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
            //builder.RegisterType<ConsoleLog>().As<ILog>();
            //builder.Register(c => new Engine(c.Resolve<ILog>(), 123));

            //builder.RegisterType<Engine>();
            //builder.RegisterType<Car>();

            // IList<T> -> List<T>
            // IList<int> -> List<int>
            builder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>));

            var container = builder.Build();

            var myList = container.Resolve<IList<int>>();
            Console.WriteLine(myList.GetType());

            //var car = container.Resolve<Car>();
            //car.Go();
        }
    }
}
