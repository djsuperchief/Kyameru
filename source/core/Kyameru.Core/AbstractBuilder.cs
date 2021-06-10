using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

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
        /// <param name="to">Valid component name.</param>
        /// <param name="headers">Dictionary of headers</param>
        /// <returns>Returns an instance of the <see cref="IToComponent"/> interface.</returns>
        protected IToComponent CreateTo(string to, Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            IToComponent response = null;
            try
            {
                response = this.GetOasis(to).CreateToComponent(headers, serviceProvider);
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
        /// <param name="isAtomic">Indicates if the route is atomic.</param>
        /// <returns>Returns an instance of the <see cref="IFromComponent"/> interface.</returns>
        protected IFromComponent CreateFrom(string from, Dictionary<string, string> headers, IServiceProvider serviceProvider, bool isAtomic = false)
        {
            IFromComponent response = null;
            try
            {               
                response = this.GetOasis(from).CreateFromComponent(headers, isAtomic, serviceProvider);
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
                response = this.GetOasis(from).CreateAtomicComponent(headers);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATING_ATOMIC, ex, "Atomic");
            }

            return response;
        }

        protected void RegisterToServices(IServiceCollection serviceCollection, string component)
        {
            try
            {
                this.GetOasis(component).RegisterTo(serviceCollection);
            }
            catch(Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_REGISTERING_SERVICES, ex, "RegisterToServices");
            }
        }

        protected void RegisterFromServices(IServiceCollection serviceCollection, string component)
        {
            try
            {
                this.GetOasis(component).RegisterFrom(serviceCollection);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_REGISTERING_SERVICES, ex, "RegisterFromServices");
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