using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
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
        private readonly List<Entities.Processable> components = new List<Entities.Processable>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteBuilder"/> class.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        public RouteBuilder(string componentUri)
        {
            this.fromUri = new RouteAttributes(componentUri);
        }

        /// <summary>
        /// Gets the processing component count.
        /// </summary>
        public int ComponentCount => this.components.Count;

        /// <summary>
        /// Adds a processing component.
        /// </summary>
        /// <param name="processComponent">Component to add.</param>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public RouteBuilder Process(IProcessComponent processComponent)
        {
            this.components.Add(Entities.Processable.Create(processComponent));

            return this;
        }

        /// <summary>
        /// Create a new process component.
        /// </summary>
        /// <typeparam name="T">Type of process component.</typeparam>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public RouteBuilder Process<T>() where T : IProcessComponent
        {
            this.components.Add(Entities.Processable.Create<T>());

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
            this.components.Add(Entities.Processable.Create(new BaseComponents.AddHeader(key, value)));
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
            this.components.Add(Entities.Processable.Create(new BaseComponents.AddHeader(key, callback)));
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
            this.components.Add(Entities.Processable.Create(new BaseComponents.AddHeader(key, callback)));
            return this;
        }

        /// <summary>
        /// Adds a to component.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri)
        {
            RouteAttributes route = new Entities.RouteAttributes(componentUri);
            return new Builder(
                this.components,
                route,
                this.fromUri);
        }
    }
}