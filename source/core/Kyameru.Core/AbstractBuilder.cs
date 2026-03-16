using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Kyameru.Core.Comms;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;

namespace Kyameru.Core
{
    /// <summary>
    /// Abstract builder class.
    /// </summary>
    /// <remarks>
    /// Will make the below more efficient but readable is better for now,
    /// optimize later.
    /// </remarks>
    public abstract class AbstractBuilder
    {
        /// <summary>
        /// Route Id.
        /// </summary>
        protected string identity;

        /// <summary>
        /// Instances of component inflators.
        /// </summary>
        protected readonly Dictionary<string, IOasis> ComponentInflators = new Dictionary<string, IOasis>();
        
        /// <summary>
        /// Instances of component event inflators.
        /// </summary>
        protected readonly Dictionary<string, IEventOasis> ComponentEventInflators = new  Dictionary<string, IEventOasis>();

        /// <summary>
        /// Value indicating if the route specifies an event and so can add a router monitor.
        /// </summary>
        protected bool HasEventRoute;
        
        /// <summary>
        /// Creates the to component.
        /// </summary>
        /// <param name="to">To component.</param>
        /// <param name="serviceProvider">DI Service provider.</param>
        /// <returns>Returns an instance of the <see cref="IToChainLink"/> interface.</returns>
        protected IToChainLink CreateTo(RouteAttributes to, IServiceProvider serviceProvider)
        {
            IToChainLink response = null;
            try
            {
                response = GetOasis(to.ComponentName).CreateToComponent(to.Headers, serviceProvider);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATION_TO, ex, "To");
            }

            return response;
        }

        /// <summary>
        /// Creates the from component.
        /// </summary>
        /// <param name="from">Valid component name.</param>
        /// <param name="headers">Dictionary of headers</param>
        /// <param name="serviceProvider">DI Service provider.</param>
        /// <returns>Returns an instance of the <see cref="IFromChainLink"/> interface.</returns>
        protected IFromChainLink CreateFrom(string from, Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            IFromChainLink response = null;
            try
            {
                response = GetOasis(from).CreateFromComponent(headers, serviceProvider);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATION_FROM, ex, "From");
            }

            return response;
        }

        /// <summary>
        /// Creates the from chain link that is event driven.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="headers"></param>
        /// <param name="bus"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        protected (IFromEventChainLink chainLink, ChannelReader<CommsMessage> messageQueue) CreateEventFrom(string from, Dictionary<string, string> headers, IKRouter bus,
            IServiceProvider serviceProvider)
        {
            IFromEventChainLink response = null;
            try
            {
                var activator = GetEventOasis(from);

                var channel = bus.Subscribe(identity);
                response = activator.CreateFromEvent(headers, serviceProvider);
                return (response, channel);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATION_FROM, ex, "FromEvent");
            }
        }

        /// <summary>
        /// Creates the scheduled component.
        /// </summary>
        /// <param name="from">Valid component name.</param>
        /// <param name="headers">Dictionary of headers</param>
        /// <param name="serviceProvider">DI Service provider.</param>
        /// <returns>Returns an instance of the <see cref="IScheduleChainLink"/> interface.</returns>
        protected IScheduleChainLink CreateScheduled(string from, Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            IScheduleChainLink response = null;
            try
            {
                response = GetOasis(from).CreateScheduleComponent(headers, serviceProvider);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(string.Format(Resources.ERROR_SCHEDULE_NOTSUPPORTED, from), ex, "Scheduled");
            }

            return response;
        }

        /// <summary>
        /// Registers to services through DI.
        /// </summary>
        /// <param name="serviceCollection">DI Service descriptors</param>
        /// <param name="component">Component to target.</param>
        protected void RegisterToServices(IServiceCollection serviceCollection, string component)
        {
            try
            {
                GetOasis(component).RegisterTo(serviceCollection);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_REGISTERING_SERVICES, ex, component);
            }
        }

        /// <summary>
        /// Registers from services through DI.
        /// </summary>
        /// <param name="serviceCollection">DI Service descriptors</param>
        /// <param name="component">Component to target.</param>
        protected void RegisterFromServices(IServiceCollection serviceCollection, string component)
        {
            try
            {
                GetOasis(component).RegisterFrom(serviceCollection);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_REGISTERING_SERVICES, ex, component);
            }
        }

        /// <summary>
        /// Registers scheduled services through DI.
        /// </summary>
        /// <param name="serviceCollection">DI Service descriptors</param>
        /// <param name="component">Component to target.</param>
        protected void RegisterScheduledServices(IServiceCollection serviceCollection, string component)
        {
            try
            {
                GetOasis(component).RegisterScheduled(serviceCollection);
            }
            catch (NotImplementedException)
            {
                throw new Exceptions.ActivationException(string.Format(Resources.ERROR_SCHEDULE_NOTSUPPORTED, component), component);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_REGISTERING_SERVICES, ex, component);
            }
        }
        
        /// <summary>
        /// Fills in the inflators
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="from">From Uri</param>
        /// <param name="to">To Uri</param>
        protected void FillInflators(IServiceCollection services, string from, List<string> to)
        {
            GetOasis(from);
            foreach (var toComponent in to)
            {
                GetOasis(toComponent);
            }

            GetEventOasis(from, true);
        }

        /// <summary>
        /// Gets the IOasis (activator) from the component.
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Returns an instance of the <see cref="IOasis"/> interface.</returns>
        private IOasis GetOasis(string component)
        {
            var componentLocation = $"$Kyameru.Component.{component}.Inflator";
            IOasis fromType = null;
            if (ComponentInflators.ContainsKey(componentLocation))
            {
                fromType = ComponentInflators[componentLocation];
            }
            else
            {
                Type createType = Type.GetType($"Kyameru.Component.{component}.Inflator, Kyameru.Component.{component}");
                if (createType == null)
                {
                    throw new ActivationException(string.Format(Resources.ERROR_ACTIVATION_FROM, component), component);    
                }
                
                fromType = (IOasis)Activator.CreateInstance(createType);
                ComponentInflators.Add(componentLocation, fromType);
            }

            return fromType;
        }

        /// <summary>
        /// Gets the event activator from the component.
        /// </summary>
        /// <param name="component">Component name.</param>
        /// <param name="initialFill">Value indicating if this is a pre-fill operation.</param>
        /// <returns>Returns an instance of the <see cref="IEventOasis"/> interface.</returns>
        private IEventOasis GetEventOasis(string component, bool initialFill = false)
        {
            var componentLocation = $"$Kyameru.Component.{component}.EventInflator";
            if (!ComponentEventInflators.TryGetValue(componentLocation, out var fromType))
            {
                Type createType = Type.GetType($"Kyameru.Component.{component}.EventInflator, Kyameru.Component.{component}");
                if (createType != null)
                {
                    fromType = (IEventOasis)Activator.CreateInstance(createType);
                    ComponentEventInflators.Add(componentLocation, fromType);
                }
                
                if (createType == null && !initialFill)
                {
                    throw new Exceptions.ActivationException(Resources.ERROR_EVENT_TRIGGER_UNSUPPORTED, "FromEvent");
                }

            }
            
            return fromType;
        }
    }
}