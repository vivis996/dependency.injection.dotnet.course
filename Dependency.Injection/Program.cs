using System;
using Autofac;

namespace Dependency.Injection
{
    public class Entity
    {
        public delegate Entity Factory();

        private static Random _random = new Random();
        private int _number;

        public Entity()
        {
            this._number = _random.Next();
        }

        public override string ToString()
        {
            return $"Test {this._number}";
        }
    }

    public class ViewModel
    {
        private readonly Entity.Factory entityFactory;

        public ViewModel(Entity.Factory entityFactory)
        {
            this.entityFactory = entityFactory;
        }

        public void Method()
        {
            var entity = entityFactory();
            Console.WriteLine(entity);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<Entity>().InstancePerDependency();
            cb.RegisterType<ViewModel>();

            var container = cb.Build();
            var vm = container.Resolve<ViewModel>();

            vm.Method();
            vm.Method();
        }
    }
}