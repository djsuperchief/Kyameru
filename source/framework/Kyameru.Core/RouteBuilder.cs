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
        private readonly List<IProcessComponent> components = new List<IProcessComponent>();

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

        public Builder To(string to, params string[] args)
        {
            return new Builder(this.from, this.components, this.SetTo(to, args));
        }

        public Builder To(string componentUri)
        {
            UriBuilder uriBuilder = new UriBuilder(componentUri);
            string query = $"Target={uriBuilder.Path}&{uriBuilder.Query.Substring(1)}";
            return new Builder(this.from, this.components, this.SetTo(
                uriBuilder.Scheme.ToFirstCaseUpper(),
                null,
                this.ParseQuery(query)));
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

        private IToComponent SetTo(string to, string[] args = null, Dictionary<string, string> headers = null)
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