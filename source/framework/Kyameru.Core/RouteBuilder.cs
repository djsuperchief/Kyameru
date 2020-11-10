using System;
using System.Collections.Generic;
using System.Linq;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;

namespace Kyameru.Core
{
    public class RouteBuilder : AbstractBuilder
    {
        private readonly Contracts.IFromComponent from;
        private readonly List<IProcessComponent> components = new List<IProcessComponent>();

        public RouteBuilder(string from, string[] args)
        {
            this.from = this.SetFrom(from, args);
        }

        public RouteBuilder(string componentUri)
        {
            Entities.RouteAttributes route = new Entities.RouteAttributes(componentUri);
            this.from = this.SetFrom(
                route.ComponentName, null,
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

        public Builder To(string to, params string[] args)
        {
            return new Builder(this.from, this.components, this.CreateTo(to, args));
        }

        public Builder To(string componentUri)
        {
            Entities.RouteAttributes route = new Entities.RouteAttributes(componentUri);
            return new Builder(this.from, this.components, this.CreateTo(
                route.ComponentName,
                null,
                route.Headers));
        }

        private Contracts.IFromComponent SetFrom(string from, string[] args = null, Dictionary<string, string> headers = null)
        {
            Contracts.IFromComponent response = null;
            try
            {
                Type fromType = Type.GetType($"Kyameru.Component.{from}.Inflator, Kyameru.Component.{from}");
                IOasis oasis = (IOasis)Activator.CreateInstance(fromType);
                if (headers == null)
                {
                    response = oasis.CreateFromComponent(args);
                }
                else
                {
                    response = oasis.CreateFromComponent(headers);
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