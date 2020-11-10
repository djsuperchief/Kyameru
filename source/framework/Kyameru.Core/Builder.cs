using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Kyameru.Core.Chain;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core
{
    public class Builder : AbstractBuilder
    {
        private readonly IFromComponent From;
        private readonly List<IToComponent> ToComponents = new List<IToComponent>();
        private readonly List<IProcessComponent> Components;
        private IErrorComponent errorComponent;

        public Builder(Contracts.IFromComponent from,
            List<IProcessComponent> components,
            Contracts.IToComponent to)
        {
            this.From = from;
            this.ToComponents.Add(to);
            this.Components = components;
        }

        public Builder To(string componentUri)
        {
            Entities.RouteAttributes route = new Entities.RouteAttributes(componentUri);
            this.ToComponents.Add(this.CreateTo(
                route.ComponentName,
                null,
                route.Headers));
            return this;
        }

        public Builder To(string to, params string[] args)
        {
            this.ToComponents.Add(this.CreateTo(to, args));
            return this;
        }

        public Builder Error(IErrorComponent errorComponent)
        {
            throw new NotImplementedException("Currently not implemented.");
        }

        public void Build(IServiceCollection services)
        {
            services.AddHostedService<Chain.From>(x =>
            {
                ILogger logger = x.GetService<ILogger<Route>>();
                logger.LogInformation(Resources.INFO_SETTINGUPROUTE);
                IChain<Routable> next = null;
                IChain<Routable> toChain = this.SetupToChain(0, logger);
                if (this.Components != null && this.Components.Count > 0)
                {
                    next = SetupChain(0, logger, toChain);
                }
                else
                {
                    next = toChain;
                }

                return new Chain.From(this.From, next, logger);
            });
        }

        private IChain<Routable> SetupChain(int i, ILogger logger, IChain<Routable> toComponents)
        {
            Chain.Process chain = new Chain.Process(logger, this.Components[i]);
            logger.LogInformation(string.Format(Resources.INFO_PROCESSINGCOMPONENT, this.Components[i].ToString()));
            if (i < this.Components.Count - 1)
            {
                chain.SetNext(this.SetupChain(++i, logger, toComponents));
            }
            else
            {
                chain.SetNext(toComponents);
            }

            return chain;
        }

        private IChain<Routable> SetupToChain(int i, ILogger logger)
        {
            Chain.To toChain = new To(logger, this.ToComponents[i]);
            logger.LogInformation(string.Format(Resources.INFO_SETUP_TO, this.ToComponents[i].ToString()));
            if (i < this.ToComponents.Count - 1)
            {
                toChain.SetNext(this.SetupToChain(++i, logger));
            }

            return toChain;
        }
    }
}