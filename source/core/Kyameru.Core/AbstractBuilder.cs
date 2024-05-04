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
    /// optimise later.
    /// </remarks>
    public abstract class AbstractBuilder
    {
        /// <summary>
        /// Creates the to component.
        /// </summary>
        /// <param name="to">To component.</param>
        /// <param name="serviceProvider">DI Service provider.</param>
        /// <returns>Returns an instance of the <see cref="IToComponent"/> interface.</returns>
        protected IToComponent CreateTo(RouteAttributes to, IServiceProvider serviceProvider)
        {
            IToComponent response = null;
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
        /// <param name="isAtomic">Indicates if the route is atomic.</param>
        /// <returns>Returns an instance of the <see cref="IFromComponent"/> interface.</returns>
        protected IFromComponent CreateFrom(string from, Dictionary<string, string> headers, IServiceProvider serviceProvider, bool isAtomic)
        {
            IFromComponent response = null;
            try
            {
                response = GetOasis(from).CreateFromComponent(headers, isAtomic, serviceProvider);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATION_FROM, ex, "From");
            }

            return response;
        }

        /// <summary>
        /// Creates the atomic component.
        /// </summary>
        /// <param name="from">Valid component name.</param>
        /// <param name="headers">Dictionary of headers</param>
        /// <returns>Returns an instance of the <see cref="IAtomicComponent"/> interface.</returns>
        protected IAtomicComponent CreateAtomic(string from, Dictionary<string, string> headers)
        {
            IAtomicComponent response = null;
            try
            {
                response = GetOasis(from).CreateAtomicComponent(headers);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATING_ATOMIC, ex, "Atomic");
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