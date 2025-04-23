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
        public RouteBuilder Process(IProcessor processComponent)
        {
            components.Add(Processable.Create(processComponent));

            return this;
        }

        /// <summary>
        /// Create a new process component.
        /// </summary>
        /// <typeparam name="T">Type of process component.</typeparam>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public RouteBuilder Process<T>() where T : IProcessor
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
            components.Add(Processable.Create(new BaseProcessors.AddHeader(key, value)));
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
            components.Add(Processable.Create(new BaseProcessors.AddHeader(key, callback)));
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
            components.Add(Processable.Create(new BaseProcessors.AddHeader(key, callback)));
            return this;
        }

        /// <summary>
        /// Adds a to component.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri) => GetBuilder().To(componentUri);

        /// <summary>
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component) => GetBuilder().When(conditional, component);


        /// <summary>
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <param name="postProcessing">Async post processing delegate</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component, Func<Routable, Task> postProcessing) =>
            GetBuilder().When(conditional, component, postProcessing);

        /// <summary>
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <param name="postProcessing">Post processing delegate</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component, Action<Routable> postProcessing) =>
            GetBuilder().When(conditional, component, postProcessing);

        /// <summary>
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <param name="postProcessing">Post processing component</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component, IProcessor postProcessing) =>
            GetBuilder().When(conditional, component, postProcessing);

        /// <summary>
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <typeparam name="T">IProcessComponent</typeparam>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When<T>(Func<Routable, bool> conditional, string component) where T : IProcessor =>
            GetBuilder().When<T>(conditional, component);

        /// <summary>
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <param name="postProcessing">Post processing component</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component, string postProcessing) =>
            GetBuilder().When(conditional, component, postProcessing);

        /// <summary>
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(string conditional, string component) =>
            GetBuilder().When(conditional, component);

        /// <summary>
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <param name="postProcessing">Post processing.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(string conditional, string component, string postProcessing) =>
            GetBuilder().When(conditional, component, postProcessing);


        /// <summary>
        /// Adds a to component with post processing.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <param name="concretePostProcessing">A component to run any post processing.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri, IProcessor concretePostProcessing) => GetBuilder().To(componentUri, concretePostProcessing);

        /// <summary>
        /// Adds a to component with post processing by DI
        /// </summary>
        /// <typeparam name="T">Type of post processing component</typeparam>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To<T>(string componentUri) where T : IProcessor => GetBuilder().To<T>(componentUri);

        /// <summary>
        /// Adds a to component with post processing by action
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <param name="action">Action to perform post processing.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri, Action<Routable> action) => GetBuilder().To(componentUri, action);

        /// <summary>
        /// Adds a to component with post processing by action
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <param name="postProcessing">Action to perform post processing.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri, Func<Routable, Task> postProcessing) => GetBuilder().To(componentUri, postProcessing);

        /// <summary>
        /// Adds a to component with post processing by action
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <param name="componentName">Name of the component to find by reflection (host assembly).</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri, string componentName) => GetBuilder().To(componentUri, componentName);

        internal Builder ToBuilder()
        {
            return GetBuilder();
        }

        private Builder GetBuilder()
        {
            return new Builder(
                components,
                fromUri,
                hostAssembly
            );
        }
    }
}