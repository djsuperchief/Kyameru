using System;
using System.Reflection;
using Kyameru.Core.Exceptions;

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
            Concrete,
            
            /// <summary>
            /// Reflection creation
            /// </summary>
            Reflection,

            /// <summary>
            /// Action delegate execution
            /// </summary>
            ActionDelegate
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <param name="target">Target component.</param>
        protected Processable(IProcessComponent target)
        {
            Invocation = InvocationType.Concrete;
            Component = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <param name="type">Type to pull from DI.</param>
        protected Processable(Type type)
        {
            Invocation = InvocationType.DI;
            ComponentType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <param name="type">Type of the component loaded via reflection.</param>
        protected Processable(string type)
        {
            Invocation = InvocationType.Reflection;
            ComponentTypeName = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <param name="action">Delegate to execute.</param>

        protected Processable(Action<Routable> action)
        {
            Invocation = InvocationType.ActionDelegate;
            Component = new ProcessableDelegate(action);
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
        /// Gets the component type name if creation is reflection.
        /// </summary>
        public string ComponentTypeName { get; private set; }

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
        /// Creates an instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <param name="component">Component name and namespace (not including app domain).</param>
        /// <returns>Returns an instance of the <see cref="Processable"/> class.</returns>
        public static Processable Create(string component)
        {
            return new Processable(component);
        }

        /// <summary>
        /// Creates an instance of the <see cref="Processable"/> class.
        /// </summary>
        /// <param name="action">Delegate to execute</param>
        /// <returns>Returns an instance of the <see cref="Processable"/> class.</returns>

        public static Processable Create(Action<Routable> action)
        {
            return new Processable(action);
        }

        /// <summary>
        /// Gets the component from either local store or service provider.
        /// </summary>
        /// <param name="provider">DI Service Provider.</param>
        /// <param name="hostAssembly">Host assembly namespace.</param>
        /// <returns>Returns an instance of the <see cref="IProcessComponent"/> class.</returns>
        public IProcessComponent GetComponent(IServiceProvider provider, Assembly hostAssembly)
        {
            switch (Invocation)
            {
                case InvocationType.Concrete:
                case InvocationType.ActionDelegate:
                    return Component;
                case InvocationType.DI:
                    return (IProcessComponent)provider.GetService(ComponentType);
                case InvocationType.Reflection:
                    return GetReflectedComponent(ComponentTypeName, hostAssembly);
            }

            throw new ComponentException(Resources.ERROR_SETUP_COMPONENT_INVOCATION);
        }

        private IProcessComponent GetReflectedComponent(string componentTypeName, Assembly hostAssembly)
        {
            var componentName = string.Concat(hostAssembly.FullName.Split(',')[0], ".", componentTypeName);
            Type componentType = hostAssembly.GetType(componentName);
            
            return Activator.CreateInstance(componentType) as IProcessComponent;
        }
    }
}