using System;
using System.Collections.Generic;
using Kyameru.Core.Chain;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core
{
    /// <summary>
    /// Builder facility.
    /// </summary>
    public class Builder : AbstractBuilder
    {
        /// <summary>
        /// List of processing components.
        /// </summary>
        private readonly List<Entities.Processable> components;

        /// <summary>
        /// List of to component uris.
        /// </summary>
        private readonly List<RouteAttributes> toUris = new List<RouteAttributes>();

        /// <summary>
        /// From URI held to construct atomic component.
        /// </summary>
        private readonly RouteAttributes fromUri;

        /// <summary>
        /// Error component.
        /// </summary>
        private IErrorComponent errorComponent;

        /// <summary>
        /// Atomic component.
        /// </summary>
        private IAtomicComponent atomicComponent;

        /// <summary>
        /// Route Id.
        /// </summary>
        private string identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        /// <param name="components">List of intermediary components.</param>
        /// <param name="to">To component.</param>
        /// <param name="fromUri">From Uri.</param>
        public Builder(
            List<Entities.Processable> components,
            RouteAttributes to,
            RouteAttributes fromUri)
        {
            this.fromUri = fromUri;
            this.toUris.Add(to);
            this.components = components;
            this.fromUri = fromUri;
        }

        /// <summary>
        /// Gets the To component count.
        /// </summary>
        public int ToComponentCount => this.toUris.Count;

        /// <summary>
        /// Gets a value indicating whether the error component will process.
        /// </summary>
        public bool WillProcessError => this.errorComponent != null;

        /// <summary>
        /// Gets a value indicating whether the route is considered to be atomic.
        /// </summary>
        public bool IsAtomic => this.atomicComponent != null;

        /// <summary>
        /// Creates a new To component chain.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri)
        {
            RouteAttributes route = new Entities.RouteAttributes(componentUri);
            this.toUris.Add(route);

            return this;
        }

        /// <summary>
        /// Creates an atomic component using the original From URI.
        /// </summary>
        /// <returns>Returns an instance of the <see cref="Builder"/> class</returns>
        public Builder Atomic()
        {
            this.atomicComponent = this.CreateAtomic(
                this.fromUri.ComponentName,
                this.fromUri.Headers);
            return this;
        }

        /// <summary>
        /// Creates an atomic component using the original From URI.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class</returns>
        public Builder Atomic(string componentUri)
        {
            RouteAttributes route = new RouteAttributes(componentUri);
            this.atomicComponent = this.CreateAtomic(
                route.ComponentName,
                route.Headers);
            return this;
        }

        /// <summary>
        /// Creates a new Error component chain.
        /// </summary>
        /// <param name="errorComponent">Error component.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder Error(IErrorComponent errorComponent)
        {
            this.errorComponent = errorComponent;
            return this;
        }

        /// <summary>
        /// Sets the identity of the route.
        /// </summary>
        /// <param name="id">Name of the route.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder Id(string id)
        {
            this.identity = id;
            return this;
        }

        /// <summary>
        /// Builds the final chain into dependency injection.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public void Build(IServiceCollection services)
        {
            this.RunComponentDiRegistration(services);
            services.AddTransient<IHostedService>(x =>
            {
                IFromComponent from = this.CreateFrom(this.fromUri.ComponentName, this.fromUri.Headers, x, this.IsAtomic);
                ILogger logger = x.GetService<ILogger<Route>>();
                logger.LogInformation(Resources.INFO_SETTINGUPROUTE);
                IChain<Routable> next = null;
                IChain<Routable> toChain = this.SetupToChain(0, logger, x);
                if (this.components != null && this.components.Count > 0)
                {
                    next = SetupChain(0, logger, toChain, x);
                }
                else
                {
                    next = toChain;
                }

                return new Chain.From(from, next, logger, this.identity);
            });
        }

        /// <summary>
        /// Runs internal DI registration in to and from components.
        /// </summary>
        private void RunComponentDiRegistration(IServiceCollection services)
        {
            this.RegisterFromServices(services, this.fromUri.ComponentName);
            for(int i = 0; i < this.toUris.Count; i++)
            {
                this.RegisterToServices(services, this.toUris[i].ComponentName);
            }
        }

        /// <summary>
        /// Sets up the processing chain
        /// </summary>
        /// <param name="i">Current count.</param>
        /// <param name="logger">Logger class.</param>
        /// <param name="toComponents">To components.</param>
        /// <param name="serviceProvider">DI service provider.</param>
        /// <returns>Returns an instance of the <see cref="IChain{T}"/> interface.</returns>
        private IChain<Routable> SetupChain(int i, ILogger logger, IChain<Routable> toComponents, IServiceProvider serviceProvider)
        {
            Process chain = new Chain.Process(logger, this.components[i].GetComponent(serviceProvider), this.GetIdentity());
            logger.LogInformation(string.Format(Resources.INFO_PROCESSINGCOMPONENT, this.components[i].ToString()));
            if (i < this.components.Count - 1)
            {
                chain.SetNext(this.SetupChain(++i, logger, toComponents, serviceProvider));
            }
            else
            {
                chain.SetNext(toComponents);
            }

            return chain;
        }

        /// <summary>
        /// Sets up the to chain.
        /// </summary>
        /// <param name="i">Current count.</param>
        /// <param name="logger">Logger class.</param>
        /// <param name="serviceProvider">DI service provider.</param>
        /// <returns>Returns an instance of the <see cref="IChain{T}"/> interface.</returns>
        private IChain<Routable> SetupToChain(int i, ILogger logger, IServiceProvider serviceProvider)
        {
            To toChain = new To(logger, this.GetToComponent(i, serviceProvider), this.GetIdentity());
            logger.LogInformation(string.Format(Resources.INFO_SETUP_TO, toChain?.ToString()));
            if (i < this.toUris.Count - 1)
            {
                toChain.SetNext(this.SetupToChain(++i, logger, serviceProvider));
            }
            else
            {
                IChain<Routable> atomic = null;
                IChain<Routable> error = null;
                if (this.atomicComponent != null)
                {
                    logger.LogInformation(string.Format(Resources.INFO_SETUP_ATOMIC, this.atomicComponent.ToString()));
                    atomic = new Atomic(logger, this.atomicComponent, this.GetIdentity());
                }

                if (this.errorComponent != null)
                {
                    logger.LogInformation(string.Format(Resources.INFO_SETUP_ERR, this.errorComponent.ToString()));
                    error = new Chain.Error(logger, this.errorComponent, this.GetIdentity());
                }

                toChain.SetNext(this.GetFinal(error, atomic));
            }

            return toChain;
        }

        private IToComponent GetToComponent(int index, IServiceProvider serviceProvider)
        {
            return this.CreateTo(this.toUris[index].ComponentName, this.toUris[index].Headers, serviceProvider);
        }

        /// <summary>
        /// Sets the correct next component.
        /// </summary>
        /// <param name="error">Component incoming.</param>
        /// <param name="atomic">Target chain.</param>
        /// <returns>Returns an instance of the <see cref="IChain{T}"/> interface.</returns>
        private IChain<Routable> GetFinal(IChain<Routable> error, IChain<Routable> atomic)
        {
            if (atomic != null && error != null)
            {
                atomic.SetNext(error);
            }
            else if (atomic == null && error != null)
            {
                atomic = error;
            }

            return atomic;
        }

        /// <summary>
        /// Gets the identity of the route.
        /// </summary>
        /// <returns>Returns either a random identity or specified.</returns>
        private string GetIdentity()
        {
            if (string.IsNullOrWhiteSpace(this.identity))
            {
                this.identity = Guid.NewGuid().ToString("N");
            }

            return this.identity;
        }
    }
}
