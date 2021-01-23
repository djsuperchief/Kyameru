using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Processable component construct.
    /// </summary>
    public class Processable
    {
        /// <summary>
        /// Enum indicating what kind of activation should be used.
        /// </summary>
        public enum InvocationType
        {
            /// <summary>
            /// Dependency injected.
            /// </summary>
            DI,

            /// <summary>
            /// Concrete implementation.
            /// </summary>
            Concrete
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <param name="target">Target component.</param>
        protected Processable(IProcessComponent target)
        {
            this.Invocation = InvocationType.Concrete;
            this.Component = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <param name="type">Type to pull from DI.</param>
        protected Processable(Type type)
        {
            this.Invocation = InvocationType.DI;
            this.ComponentType = type;
        }

        /// <summary>
        /// Gets the invocation type.
        /// </summary>
        public InvocationType Invocation { get; private set; }

        /// <summary>
        /// Gets the component type.
        /// </summary>
        public Type ComponentType { get; private set; }

        /// <summary>
        /// Gets the component.
        /// </summary>
        public IProcessComponent Component { get; private set; }

        /// <summary>
        /// Creates an instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        /// <returns>Returns an instance of the <see cref="Processable"/> class.</returns>
        public static Processable Create<T>() where T : IProcessComponent
        {
            return new Processable(typeof(T));
        }

        /// <summary>
        /// Creates an instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <param name="component">Concrete component.</param>
        /// <returns>Returns an instance of the <see cref="Processable"/> class.</returns>
        public static Processable Create(IProcessComponent component)
        {
            return new Processable(component);
        }

        /// <summary>
        /// Gets the component from either local store or service provider.
        /// </summary>
        /// <param name="provider">DI Service Provider.</param>
        /// <returns>Returns an instance of the <see cref="IProcessComponent"/> class.</returns>
        public IProcessComponent GetComponent(IServiceProvider provider)
        {
            if (this.Invocation == InvocationType.Concrete)
            {
                return this.Component;
            }
            else
            {
                return (IProcessComponent)provider.GetService(this.ComponentType);
            }
        }
    }
}