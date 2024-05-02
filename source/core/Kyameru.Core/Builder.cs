using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private readonly List<Processable> components;

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
        /// Value indicating whether exceptions should be raised from the route.
        /// False indicates framework should "swallow" the exception but still log it.
        /// </summary>
        private bool raiseExceptions;

        /// <summary>
        /// Route Id.
        /// </summary>
        private string identity;

        /// <summary>
        /// Used for when reflection is needed for host assembly reflection.
        /// </summary>
        private Assembly hostAssmebly;

        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        /// <param name="components">List of intermediary components.</param>
        /// <param name="to">To component.</param>
        /// <param name="fromUri">From Uri.</param>
        /// <param name="callingAssembly">Calling assembly namespace</param>
        public Builder(
            List<Processable> components,
            RouteAttributes to,
            RouteAttributes fromUri,
            Assembly callingAssembly = null)
        {
            this.fromUri = fromUri;
            toUris.Add(to);
            this.components = components;
            this.fromUri = fromUri;
            raiseExceptions = false;
            hostAssmebly = callingAssembly;
        }

        /// <summary>
        /// Gets the To component count.
        /// </summary>
        public int ToComponentCount => toUris.Count;

        /// <summary>
        /// Gets a value indicating whether the error component will process.
        /// </summary>
        public bool WillProcessError => errorComponent != null;

        /// <summary>
        /// Gets a value indicating whether the route is considered to be atomic.
        /// </summary>
        public bool IsAtomic => atomicComponent != null;

        /// <summary>
        /// Creates a new To component chain.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri)
        {
            var route = new RouteAttributes(componentUri);
            toUris.Add(route);

            return this;
        }

        /// <summary>
        /// Creates a new To component chain.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <param name="concretePostProcessing">A component to run any post processing.
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder To(string componentUri, IProcessComponent concretePostProcessing)
        {
            var postProcessComponent = Processable.Create(concretePostProcessing);
            var route = new RouteAttributes(componentUri, postProcessComponent);
            toUris.Add(route);

            return this;
        }

        /// <summary>
        /// Creates an atomic component using the original From URI.
        /// </summary>
        /// <returns>Returns an instance of the <see cref="Builder"/> class</returns>
        public Builder Atomic()
        {
            atomicComponent = CreateAtomic(
                fromUri.ComponentName,
                fromUri.Headers);
            return this;
        }

        /// <summary>
        /// Creates an atomic component using the original From URI.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class</returns>
        public Builder Atomic(string componentUri)
        {
            var route = new RouteAttributes(componentUri);
            atomicComponent = CreateAtomic(
                route.ComponentName,
                route.Headers);
            return this;
        }

        /// <summary>
        /// Creates a new Error component chain.
        /// </summary>
        /// <param name="component">Error component.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder Error(IErrorComponent component)
        {
            errorComponent = component;
            return this;
        }

        /// <summary>
        /// Sets the identity of the route.
        /// </summary>
        /// <param name="id">Name of the route.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> class.</returns>
        public Builder Id(string id)
        {
            identity = id;
            return this;
        }

        /// <summary>
        /// Indicates that the framework will bubble a route exception up to consumer.
        /// </summary>
        /// <returns></returns>
        public Builder RaiseExceptions()
        {
            raiseExceptions = true;
            return this;
        }

        /// <summary>
        /// Builds the final chain into dependency injection.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public void Build(IServiceCollection services)
        {
            if (hostAssmebly == null && ContainsReflectionComponents())
            {
                hostAssmebly = Assembly.GetCallingAssembly();
            }

            BuildKyameru(services);
        }

        private void BuildKyameru(IServiceCollection services)
        {
            RunComponentDiRegistration(services);
            services.AddTransient<IHostedService>(x =>
            {
                var from = CreateFrom(fromUri.ComponentName, fromUri.Headers, x, IsAtomic);
                ILogger logger = x.GetService<ILogger<Route>>();
                logger.LogInformation(Resources.INFO_SETTINGUPROUTE);
                IChain<Routable> next = null;
                var toChain = SetupToChain(0, logger, x);
                if (components != null && components.Count > 0)
                {
                    next = SetupChain(0, logger, toChain, x);
                }
                else
                {
                    next = toChain;
                }

                return new From(from, next, logger, identity, IsAtomic, raiseExceptions);
            });
        }

        /// <summary>
        /// Runs internal DI registration in to and from components.
        /// </summary>
        private void RunComponentDiRegistration(IServiceCollection services)
        {
            RegisterFromServices(services, fromUri.ComponentName);
            foreach (var to in toUris)
            {
                RegisterToServices(services, to.ComponentName);
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
            var chain = new Process(logger, components[i].GetComponent(serviceProvider, hostAssmebly), GetIdentity());
            logger.LogInformation(string.Format(Resources.INFO_PROCESSINGCOMPONENT, components[i]));
            if (i < components.Count - 1)
            {
                chain.SetNext(SetupChain(++i, logger, toComponents, serviceProvider));
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
            var toChain = new To(logger, GetToComponent(i, serviceProvider), GetIdentity());
            logger.LogInformation(string.Format(Resources.INFO_SETUP_TO, toChain?.ToString()));
            if (i < toUris.Count - 1)
            {
                toChain.SetNext(SetupToChain(++i, logger, serviceProvider));
            }
            else
            {
                IChain<Routable> atomic = null;
                IChain<Routable> error = null;
                if (atomicComponent != null)
                {
                    logger.LogInformation(string.Format(Resources.INFO_SETUP_ATOMIC, atomicComponent.ToString()));
                    atomic = new Atomic(logger, atomicComponent, GetIdentity());
                }

                if (errorComponent != null)
                {
                    logger.LogInformation(string.Format(Resources.INFO_SETUP_ERR, errorComponent.ToString()));
                    error = new Chain.Error(logger, errorComponent, GetIdentity());
                }

                toChain.SetNext(GetFinal(error, atomic));
            }

            return toChain;
        }

        private IToComponent GetToComponent(int index, IServiceProvider serviceProvider)
        {
            return CreateTo(toUris[index].ComponentName, toUris[index].Headers, serviceProvider);
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
            if (string.IsNullOrWhiteSpace(identity))
            {
                identity = Guid.NewGuid().ToString("N");
            }

            return identity;
        }

        private bool ContainsReflectionComponents()
        {
            return components.Count(x => x.Invocation == Processable.InvocationType.Reflection) > 0;
        }
    }
}
