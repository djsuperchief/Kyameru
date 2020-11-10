using Kyameru.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core
{
    public abstract class AbstractBuilder
    {
        protected IToComponent CreateTo(string to, string[] args = null, Dictionary<string, string> headers = null)
        {
            IToComponent response = null;
            try
            {
                Type fromType = Type.GetType($"Kyameru.Component.{to}.Inflator, Kyameru.Component.{to}");
                IOasis oasis = (IOasis)Activator.CreateInstance(fromType);
                if (headers == null)
                {
                    response = oasis.CreateToComponent(args);
                }
                else
                {
                    response = oasis.CreateToComponent(headers);
                }
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATION_FROM, ex);
            }

            return response;
        }
    }
}