using Kyameru.Core.Contracts;
using System;
using System.Collections.Generic;

namespace Kyameru.Core
{
    public abstract class AbstractBuilder
    {
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