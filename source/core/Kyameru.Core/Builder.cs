using System;
using System.Collections.Generic;
using Kyameru.Core.Chain;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core
{
    /// <summary>
    /// Builder facility.
    /// </summary>
    public class Builder : AbstractBuilder
    {
        /// <summary>
        /// From component.
        /// </summary>
        private readonly IFromComponent from;

        /// <summary>
        /// List of to components.
        /// </summary>
        private readonly List<IToComponent> toComponents = new List<IToComponent>();

        /// <summary>
        /// List of processing components.
        /// </summary>
        private readonly List<IProcessComponent> components;

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
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        /// <param name="from">From component.</param>
        /// <param name="components">List of intermediary components.</param>
        /// <param name="to">To component.</param>
        public Builder(
            Contracts.IFromComponent from,
            List<IProcessComponent> components,
            Contracts.IToComponent to,
            RouteAttributes fromUri)
        {
            this.from = from;
            this.toComponents.Add(to);
            this.components = components;
        }

        /// <summary>
        /// Gets the To component count.
        /// </summary>
        public int ToComponentCount => this.toComponents.Count;

        /// <summary>
        /// Gets a value indicating whether the error component will process.
        /// </summary>
        public bool WillProcessError => this.errorComponent != null;

        /// <summary>
        /// Creates a new To component chain.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri)
        {
            Entities.RouteAttributes route = new Entities.RouteAttributes(componentUri);
            this.toComponents.Add(this.CreateTo(
                route.ComponentName,
                route.Headers));
            return this;
        }

        /// <summary>
        /// Creates an atomic component using the original From URI
        /// </summary>
        /// <returns><Returns an instance of the <see cref="Builder"/> class</returns>
        public Builder Atomic()
        {
            this.atomicComponent = this.CreateAtomic(
                this.fromUri.ComponentName,
                this.fromUri.Headers);
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
        /// Builds the final chain into dependency injection.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public void Build(IServiceCollection services)
        {
            services.AddHostedService<Chain.From>(x =>
            {
                ILogger logger = x.GetService<ILogger<Route>>();
                logger.LogInformation(Resources.INFO_SETTINGUPROUTE);
                IChain<Routable> next = null;
                IChain<Routable> toChain = this.SetupToChain(0, logger);
                if (this.components != null && this.components.Count > 0)
                {
                    next = SetupChain(0, logger, toChain);
                }
                else
                {
                    next = toChain;
                }

                return new Chain.From(this.from, next, logger);
            });
        }

        /// <summary>
        /// Sets up the processing chain
        /// </summary>
        /// <param name="i">Current count.</param>
        /// <param name="logger">Logger class.</param>
        /// <param name="toComponents">To components.</param>
        /// <returns>Returns an instance of the <see cref="IChain{T}"/> interface.</returns>
        private IChain<Routable> SetupChain(int i, ILogger logger, IChain<Routable> toComponents)
        {
            Chain.Process chain = new Chain.Process(logger, this.components[i]);
            logger.LogInformation(string.Format(Resources.INFO_PROCESSINGCOMPONENT, this.components[i].ToString()));
            if (i < this.components.Count - 1)
            {
                chain.SetNext(this.SetupChain(++i, logger, toComponents));
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
        /// <returns>Returns an instance of the <see cref="IChain{T}"/> interface.</returns>
        private IChain<Routable> SetupToChain(int i, ILogger logger)
        {
            Chain.To toChain = new To(logger, this.toComponents[i]);
            logger.LogInformation(string.Format(Resources.INFO_SETUP_TO, this.toComponents[i].ToString()));
            if (i < this.toComponents.Count - 1)
            {
                toChain.SetNext(this.SetupToChain(++i, logger));
            }
            else 
            {
                

                if (this.errorComponent != null)
                {
                    logger.LogInformation(string.Format(Resources.INFO_SETUP_ERR, this.errorComponent.ToString()));
                    toChain.SetNext(new Chain.Error(logger, this.errorComponent));
                }
            }

            return toChain;
        }
    }
}