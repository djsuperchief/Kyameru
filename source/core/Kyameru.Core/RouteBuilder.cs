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
        /// From component.
        /// </summary>
        private readonly Contracts.IFromComponent from;

        /// <summary>
        /// List of intermediary components.
        /// </summary>
        private readonly List<IProcessComponent> components = new List<IProcessComponent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteBuilder"/> class.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        public RouteBuilder(string componentUri)
        {
            Entities.RouteAttributes route = new Entities.RouteAttributes(componentUri);
            this.from = this.SetFrom(
                route.ComponentName,
                route.Headers);
        }

        /// <summary>
        /// Gets a value indicating whether the from component is valid.
        /// </summary>
        public bool FromValid => this.from != null;

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
            this.components.Add(processComponent);

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
            this.components.Add(new BaseComponents.AddHeader(key, value));
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
            this.components.Add(new BaseComponents.AddHeader(key, callback));
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
            this.components.Add(new BaseComponents.AddHeader(key, callback));
            return this;
        }

        /// <summary>
        /// Adds a to component.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri)
        {
            Entities.RouteAttributes route = new Entities.RouteAttributes(componentUri);
            return new Builder(
                this.from,
                this.components,
                this.CreateTo(route.ComponentName, route.Headers));
        }

        /// <summary>
        /// Sets the from component.
        /// </summary>
        /// <param name="from">Valid from name.</param>
        /// <param name="headers">Dictionary of headers.</param>
        /// <returns>Returns an instance of the <see cref="IFromComponent"/> interface.</returns>
        private Contracts.IFromComponent SetFrom(string from, Dictionary<string, string> headers)
        {
            Contracts.IFromComponent response = null;
            try
            {
                Type fromType = Type.GetType($"Kyameru.Component.{from}.Inflator, Kyameru.Component.{from}");
                IOasis oasis = (IOasis)Activator.CreateInstance(fromType);
                response = oasis.CreateFromComponent(headers);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATION_FROM, ex);
            }

            return response;
        }
    }
}