using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Kyameru.Core.Entities;

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
        /// Gets the IOasis (activator) from the component.
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Returns an instance of the <see cref="IOasis"/> interface.</returns>
        private IOasis GetOasis(string component)
        {
            Type fromType = Type.GetType($"Kyameru.Component.{component}.Inflator, Kyameru.Component.{component}");
            return (IOasis)Activator.CreateInstance(fromType);
        }
    }
}