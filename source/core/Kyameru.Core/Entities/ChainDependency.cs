using System;
using System.Collections.Generic;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Class to hold chain dependencies until registration time.
    /// </summary>
    internal class ChainDependency
    {
        public Guid Id { get; private set; }

        public Type Contract { get; private set; }

        public Type Implementation { get; private set; }

        //public Func<object> ImplementationFactory { get; private set; }

        public Func<IServiceProvider, object, object> ImplementationFactory { get; set; }

        private ChainDependency(Guid id, Type contract, Type implementation)
        {
            Id = id;
            Contract = contract;
            Implementation = implementation;
        }

        private ChainDependency(Guid id, Type contract, Func<IServiceProvider, object, object> implementationFactory)
        {
            Id = id;
            Contract = contract;
            ImplementationFactory = implementationFactory;
        }

        public static ChainDependency Create(Guid id, Type contract, Type implementation) =>
            new ChainDependency(id, contract, implementation);

        public static ChainDependency Create<TContract>(Guid id, Func<TContract> implementationFactory)
        {
            var factory = Box<TContract>(implementationFactory, id);

            return new ChainDependency(id, typeof(TContract), factory);
        }


        public static ChainDependency Create<TContract>(Guid id, Func<IServiceProvider, Guid, TContract> implementationFactory) =>
            new ChainDependency(id, typeof(TContract), Box(implementationFactory));

        private static Func<IServiceProvider, object, object> Box<T>(Func<IServiceProvider, Guid, T> factory)
        {
            return (serviceProvider, o) => factory(serviceProvider, (Guid)o);
        }

        private static Func<IServiceProvider, object, object> Box<T>(Func<T> factory, Guid identifier) => (serviceProvider, o) => factory();
    }
}