using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kyameru.Core.Chain;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Enums;
using Kyameru.Core.Exceptions;
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
        private Assembly hostAssembly;

        private Schedule schedule;

        // /// <summary>
        // /// Initializes a new instance of the <see cref="Builder"/> class.
        // /// </summary>
        // /// <param name="components">List of intermediary components.</param>
        // /// <param name="to">To component.</param>
        // /// <param name="fromUri">From Uri.</param>
        // /// <param name="callingAssembly">Calling assembly namespace</param>
        // internal Builder(
        //     List<Processable> components,
        //     RouteAttributes to,
        //     RouteAttributes fromUri,
        //     Assembly callingAssembly = null)
        // {
        //     this.fromUri = fromUri;
        //     toUris.Add(to);
        //     this.components = components;
        //     this.fromUri = fromUri;
        //     raiseExceptions = false;
        //     hostAssembly = callingAssembly;
        // }

        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        /// <param name="components">List of intermediary components.</param>
        /// <param name="fromUri">From Uri.</param>
        /// <param name="callingAssembly">Calling assembly namespace</param>
        internal Builder(
            List<Processable> components,
            RouteAttributes fromUri,
            Assembly callingAssembly = null
        )
        {
            this.fromUri = fromUri;
            this.components = components;
            this.fromUri = fromUri;
            raiseExceptions = false;
            hostAssembly = callingAssembly;
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
        /// Gets a value indicating whether the route is on a schedule.
        /// </summary>
        public bool IsScheduled => schedule != null;

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
        /// Adds a conditional to component.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component)
        {
            var route = new RouteAttributes(new DefaultConditional(conditional), component);
            toUris.Add(route);
            return this;
        }

        /// <summary>
        /// Adds a conditional to component with post processing.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <param name="postProcessing">Async post processing delegate</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component, Func<Routable, Task> postProcessing)
        {
            var postProcessingComponent = Processable.Create(postProcessing);
            var route = new RouteAttributes(new DefaultConditional(conditional), component, postProcessingComponent);
            toUris.Add(route);
            return this;
        }

        /// <summary>
        /// Adds a conditional to component with post processing.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <param name="postProcessing">Post processing delegate</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component, Action<Routable> postProcessing)
        {
            var postProcessingComponent = Processable.Create(postProcessing);
            AddToConditionalProcessing(conditional, component, postProcessingComponent);
            return this;
        }

        /// <summary>
        /// Adds a conditional to component with post processing.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <param name="postProcessing">Post processing component</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component, IProcessComponent postProcessing)
        {
            var postProcessingComponent = Processable.Create(postProcessing);
            AddToConditionalProcessing(conditional, component, postProcessingComponent);
            return this;
        }

        /// <summary>
        /// Adds a conditional to component with post processing.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <typeparam name="T">IProcessComponent</typeparam>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When<T>(Func<Routable, bool> conditional, string component) where T : IProcessComponent
        {
            var postProcessingComponent = Processable.Create<T>();
            AddToConditionalProcessing(conditional, component, postProcessingComponent);
            return this;
        }

        /// <summary>
        /// Adds a conditional to component with post processing.
        /// </summary>
        /// <param name="conditional">Condition to run.</param>
        /// <param name="component">To component.</param>
        /// <param name="postProcessing">Post processing component</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(Func<Routable, bool> conditional, string component, string postProcessing)
        {
            var postProcessingComponent = Processable.Create(postProcessing);
            AddToConditionalProcessing(conditional, component, postProcessingComponent);
            return this;
        }

        /// <summary>
        /// Adds a conditional to component with post processing.
        /// </summary>
        /// <param name="conditional">Conditional Component.</param>
        /// <param name="component">To Component.</param>
        /// <returns>Returns an instance of the <see cref="Builder"/> type.</returns>
        public Builder When(string conditional, string component)
        {
            var whenConditional = GetReflectedConditionalComponent(conditional, hostAssembly);
            AddToConditionalProcessing(whenConditional, component);
            return this;
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
            AddToPostProcessing(componentUri, postProcessComponent);
            return this;
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
            AddToPostProcessing(componentUri, postProcessComponent);
            return this;
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
            AddToPostProcessing(componentUri, postProcessComponent);
            return this;
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
            AddToPostProcessing(componentUri, postProcessComponent);
            return this;
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
            AddToPostProcessing(componentUri, postProcessComponent);
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
        /// Schedules the route to trigger at every <see cref="TimeUnit"/>.
        /// </summary>
        /// <param name="unit">Unit of available time.</param>
        /// <param name="value">Value of time unit</param>
        public Builder ScheduleEvery(TimeUnit unit, int value = 1)
        {
            AddSchedule(unit, value, true);
            return this;
        }

        /// <summary>
        /// Schedules the route to trigger at a specific <see cref="TimeUnit"/>.
        /// </summary>
        /// <param name="unit">Time unit</param>
        /// <param name="value">Value between 0 and max for time unit.</param>
        /// <returns></returns>
        public Builder ScheduleAt(TimeUnit unit, int value)
        {
            AddSchedule(unit, value, false);
            return this;
        }

        /// <summary>
        /// Builds the final chain into dependency injection.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public void Build(IServiceCollection services)
        {
            if (hostAssembly == null && ContainsReflectionComponents())
            {
                hostAssembly = Assembly.GetCallingAssembly();
            }

            BuildKyameru(services);
        }

        private void BuildKyameru(IServiceCollection services)
        {
            RunComponentDiRegistration(services);
            services.AddTransient<IHostedService>(x =>
            {

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

                if (schedule == null)
                {
                    var from = CreateFrom(fromUri.ComponentName, fromUri.Headers, x, IsAtomic);
                    return new From(from, next, logger, identity, IsAtomic, raiseExceptions);
                }
                else
                {
                    var scheduled = CreateScheduled(fromUri.ComponentName, fromUri.Headers, x, IsAtomic);
                    return new Scheduled(scheduled, next, logger, identity, IsAtomic, raiseExceptions, schedule);
                }


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
            if (IsScheduled)
            {
                RegisterScheduledServices(services, fromUri.ComponentName);
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
            var chain = new Process(logger, components[i].GetComponent(serviceProvider, hostAssembly), GetIdentity());
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
            To toChain = null;
            var component = GetToComponent(i, serviceProvider);
            if (toUris[i].HasPostprocessing)
            {

                toChain = new To(logger, component, toUris[i].PostProcessingComponent.GetComponent(serviceProvider, hostAssembly),
                    GetIdentity(), toUris[i].Condition);
            }
            else
            {
                toChain = new To(logger, component, GetIdentity(), toUris[i].Condition);
            }

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
            return CreateTo(toUris[index], serviceProvider);
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
            return components.Count(x => x.Invocation == Processable.InvocationType.Reflection) > 0
                || toUris.Count(x => x.HasPostprocessing) > 0;
        }

        private void AddToPostProcessing(string componentUri, Processable postProcessComponent)
        {
            var route = new RouteAttributes(componentUri, postProcessComponent);
            toUris.Add(route);
        }

        private void AddToConditionalProcessing(Func<Routable, bool> conditional, string componentUri, Processable postProcessComponent)
        {
            var route = new RouteAttributes(new DefaultConditional(conditional), componentUri, postProcessComponent);
            toUris.Add(route);
        }

        private void AddToConditionalProcessing(IConditionalComponent conditional, string componentUri)
        {
            var route = new RouteAttributes(conditional, componentUri);
            toUris.Add(route);
        }

        private void AddSchedule(TimeUnit unit, int value, bool isEvery)
        {
            if (schedule != null)
            {
                throw new CoreException(Resources.ERROR_SCHEDULE_ALREADY_DEFINED);
            }

            schedule = new Schedule(unit, value, false);
        }

        private IConditionalComponent GetReflectedConditionalComponent(string componentTypeName, Assembly hostAssembly)
        {
            var componentName = string.Concat(hostAssembly.FullName.Split(',')[0], ".", componentTypeName);
            Type componentType = hostAssembly.GetType(componentName);
            IConditionalComponent response = null;
            try
            {
                response = Activator.CreateInstance(componentType) as IConditionalComponent;

            }
            catch
            {
                var componentError = string.Format(Resources.ERROR_COMPONENT_NOT_FOUND, componentTypeName);
                throw new Exceptions.CoreException(string.Format(Resources.ERROR_ACTIVATION_TO, componentError));
            }

            return response;
        }
    }
}
