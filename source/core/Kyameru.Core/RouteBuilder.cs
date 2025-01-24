using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Core
{
    /// <summary>
    /// Route builder.
    /// </summary>
    public class RouteBuilder : AbstractBuilder
    {
        /// <summary>
        /// From URI held to construct the atomic component.
        /// </summary>
        private readonly RouteAttributes fromUri;

        /// <summary>
        /// List of intermediary components.
        /// </summary>
        private readonly List<Processable> components = new List<Processable>();

        /// <summary>
        /// Host assembly namespace.
        /// </summary>
        private readonly Assembly hostAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteBuilder"/> class.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        public RouteBuilder(string componentUri)
        {
            fromUri = new RouteAttributes(componentUri);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteBuilder"/> class.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <param name="hostAssembly">Assembly name</param>
        internal RouteBuilder(string componentUri, Assembly hostAssembly)
        {
            fromUri = new RouteAttributes(componentUri);
            this.hostAssembly = hostAssembly;
        }

        /// <summary>
        /// Gets the processing component count.
        /// </summary>
        public int ComponentCount => components.Count;

        /// <summary>
        /// Adds a processing component.
        /// </summary>
        /// <param name="processComponent">Component to add.</param>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public RouteBuilder Process(IProcessComponent processComponent)
        {
            components.Add(Processable.Create(processComponent));

            return this;
        }

        /// <summary>
        /// Create a new process component.
        /// </summary>
        /// <typeparam name="T">Type of process component.</typeparam>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public RouteBuilder Process<T>() where T : IProcessComponent
        {
            components.Add(Processable.Create<T>());

            return this;
        }

        /// <summary>
        /// Create a new process component.
        /// </summary>
        /// <param name="typeName">Component to add (namespace and not including app domain).</param>
        /// <returns><returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns></returns>
        public RouteBuilder Process(string typeName)
        {
            components.Add(Processable.Create(typeName));

            return this;
        }

        /// <summary>
        /// Create a new process component.
        /// </summary>
        /// <param name="processAction">Action delegate for processing.</param>
        /// <returns><returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns></returns>
        public RouteBuilder Process(Action<Routable> processAction)
        {
            components.Add(Processable.Create(processAction));

            return this;
        }

        /// <summary>
        /// Create a new process component.
        /// </summary>
        /// <param name="processAction">Action delegate for processing.</param>
        /// <returns><returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns></returns>
        public RouteBuilder Process(Func<Routable, Task> processAction)
        {
            components.Add(Processable.Create(processAction));

            return this;
        }

        /// <summary>
        /// Adds a header
        /// </summary>
        /// <param name="key">Header key.</param>
        /// <param name="value">Header value.</param>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public RouteBuilder AddHeader(string key, string value)
        {
            components.Add(Processable.Create(new BaseComponents.AddHeader(key, value)));
            return this;
        }

        /// <summary>
        /// Adds a header.
        /// </summary>
        /// <param name="key">Header key.</param>
        /// <param name="callback">Header callback.</param>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public RouteBuilder AddHeader(string key, Func<string> callback)
        {
            components.Add(Processable.Create(new BaseComponents.AddHeader(key, callback)));
            return this;
        }

        /// <summary>
        /// Adds a header.
        /// </summary>
        /// <param name="key">Header key.</param>
        /// <param name="callback">Header callback.</param>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public RouteBuilder AddHeader(string key, Func<Routable, string> callback)
        {
            components.Add(Processable.Create(new BaseComponents.AddHeader(key, callback)));
            return this;
        }

        /// <summary>
        /// Adds a to component.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri)
        {
            var route = new RouteAttributes(componentUri);
            return new Builder(
                components,
                route,
                fromUri,
                hostAssembly);
        }

        /// <summary>
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component)
        {
            var route = new RouteAttributes(conditional, component);
            return new Builder(
                components,
                route,
                fromUri,
                hostAssembly);
        }

        /// <summary>
        /// Adds a to component with post processing.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <param name="concretePostProcessing">A component to run any post processing.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri, IProcessComponent concretePostProcessing)
        {
            var postProcessComponent = Processable.Create(concretePostProcessing);
            return GetBuilder(componentUri, postProcessComponent);
        }

        /// <summary>
        /// Adds a to component with post processing by DI
        /// </summary>
        /// <typeparam name="T">Type of post processing component</typeparam>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To<T>(string componentUri) where T : IProcessComponent
        {
            var postProcessComponent = Processable.Create<T>();
            return GetBuilder(componentUri, postProcessComponent);
        }

        /// <summary>
        /// Adds a to component with post processing by action
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <param name="action">Action to perform post processing.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri, Action<Routable> action)
        {
            var postProcessComponent = Processable.Create(action);
            return GetBuilder(componentUri, postProcessComponent);
        }

        /// <summary>
        /// Adds a to component with post processing by action
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <param name="postProcessing">Action to perform post processing.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri, Func<Routable, Task> postProcessing)
        {
            var postProcessComponent = Processable.Create(postProcessing);
            return GetBuilder(componentUri, postProcessComponent);
        }

        /// <summary>
        /// Adds a to component with post processing by action
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <param name="componentName">Name of the component to find by reflection (host assembly).</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri, string componentName)
        {
            var postProcessComponent = Processable.Create(componentName);
            return GetBuilder(componentUri, postProcessComponent);
        }

        private Builder GetBuilder(string componentUri, Processable postProcessComponent)
        {
            var route = new RouteAttributes(componentUri, postProcessComponent);
            return new Builder(
                components,
                route,
                fromUri,
                hostAssembly
            );
        }
    }
}