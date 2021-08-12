using System;
using Autofac;

namespace Dependency.Injection
{
    internal class Service
    {
        internal string DoSomething(int value)
        {
            return $"I have {value}";
        }
    }

    internal class DomainObject
    {
        private Service _service;
        private int _value;

        public delegate DomainObject Factory(int value);

        public DomainObject(Service service, int value)
        {
            this._service = service;
            this._value = value;
        }

        public override string ToString()
        {
            return this._service.DoSomething(this._value);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Service>();
            builder.RegisterType<DomainObject>();

            var container = builder.Build();

            var dobj = container.Resolve<DomainObject>(
                new PositionalParameter(1, 543)
            );
            Console.WriteLine(dobj);

            //

            var factory = container.Resolve<DomainObject.Factory>();
            var dobj2 = factory(42);
            Console.WriteLine(dobj2);
        }
    }
}
