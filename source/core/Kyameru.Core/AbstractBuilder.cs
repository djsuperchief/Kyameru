using Kyameru.Core.Contracts;
using System;
using System.Collections.Generic;

namespace Kyameru.Core
{
    /// <summary>
    /// Abstract builder class.
    /// </summary>
    public abstract class AbstractBuilder
    {
        /// <summary>
        /// Creates the to component.
        /// </summary>
        /// <param name="to">Valid component name.</param>
        /// <param name="headers">Dictionary of headers</param>
        /// <returns>Returns an instance of the <see cref="IToComponent"/> interface.</returns>
        protected IToComponent CreateTo(string to, Dictionary<string, string> headers)
        {
            IToComponent response = null;
            try
            {
                Type fromType = Type.GetType($"Kyameru.Component.{to}.Inflator, Kyameru.Component.{to}");
                IOasis oasis = (IOasis)Activator.CreateInstance(fromType);
                response = oasis.CreateToComponent(headers);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATION_FROM, ex);
            }

            return response;
        }
    }
}