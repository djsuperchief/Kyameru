using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Core
{
    public class RouteBuilder : AbstractBuilder
    {
        private readonly Contracts.IFromComponent from;
        private readonly List<IProcessComponent> components = new List<IProcessComponent>();

        public RouteBuilder(string componentUri)
        {
            Entities.RouteAttributes route = new Entities.RouteAttributes(componentUri);
            this.from = this.SetFrom(
                route.ComponentName,
                route.Headers);
        }

        public RouteBuilder Process(IProcessComponent processComponent)
        {
            this.components.Add(processComponent);

            return this;
        }

        public RouteBuilder AddHeader(string key, string value)
        {
            this.components.Add(new BaseComponents.AddHeader(key, value));
            return this;
        }

        public RouteBuilder AddHeader(string key, Func<string> callback)
        {
            this.components.Add(new BaseComponents.AddHeader(key, callback));
            return this;
        }

        public RouteBuilder AddHeader(string key, Func<Routable, string> callback)
        {
            this.components.Add(new BaseComponents.AddHeader(key, callback));
            return this;
        }

        public Builder To(string componentUri)
        {
            Entities.RouteAttributes route = new Entities.RouteAttributes(componentUri);
            return new Builder(this.from, this.components, this.CreateTo(
                route.ComponentName,
                route.Headers));
        }

        private Contracts.IFromComponent SetFrom(string from, Dictionary<string, string> headers)
        {
            Contracts.IFromComponent response = null;
            try
            {
                Type fromType = Type.GetType($"Kyameru.Component.{from}.Inflator, Kyameru.Component.{from}");
                IOasis oasis = (IOasis)Activator.CreateInstance(fromType);
                response = oasis.CreateFromComponent(headers);
                
            }
            catch (Exception ex)
            {
                throw new Exceptions.ActivationException(Resources.ERROR_ACTIVATION_FROM, ex);
            }

            return response;
        }
    }
}