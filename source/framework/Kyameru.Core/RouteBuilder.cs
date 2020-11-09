using System;
using System.Collections.Generic;
using System.Linq;
using Kyameru.Core.Contracts;
using Kyameru.Core.Extensions;

namespace Kyameru.Core
{
    public class RouteBuilder
    {
        private readonly Contracts.IFromComponent from;
        private List<IProcessComponent> components;

        public RouteBuilder(string from, string[] args)
        {
            this.from = this.SetFrom(from, args);
        }

        public RouteBuilder(string componentUri)
        {
            UriBuilder uriBuilder = new UriBuilder(componentUri);
            string query = $"Target={uriBuilder.Path}&{uriBuilder.Query.Substring(1)}";
            this.from = this.SetFrom(
                uriBuilder.Scheme.ToFirstCaseUpper(), null,
                this.ParseQuery(query));
        }

        public RouteBuilder Process(IProcessComponent processComponent)
        {
            if (this.components == null)
            {
                this.components = new List<IProcessComponent>();
            }

            this.components.Add(processComponent);

            return this;
        }

        public Builder To(string to, params string[] args)
        {
            Type toType = Type.GetType($"Kyameru.Component.{to}.Inflator, Kyameru.Component.{to}");
            IOasis oasis = (IOasis)Activator.CreateInstance(toType);
            IToComponent toComponent = oasis.CreateToComponent(args);
            return new Builder(this.from, this.components, toComponent);
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

        /// <summary>
        /// Do not want to bring in an entire library to do this.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private Dictionary<string, string> ParseQuery(string query)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            string[] parts = query.Split('&');
            for (int i = 0; i < parts.Length; i++)
            {
                response.Add(parts[i].Split('=')[0], parts[i].Split('=')[1]);
            }

            return response;
        }
    }
}