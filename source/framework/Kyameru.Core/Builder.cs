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
    public class Builder
    {
        private readonly IFromComponent From;
        private readonly IToComponent To;
        private readonly List<IProcessComponent> Components;
        private IErrorComponent errorComponent;

        public Builder(Contracts.IFromComponent from,
            List<IProcessComponent> components,
            Contracts.IToComponent to)
        {
            this.From = from;
            this.To = to;
            this.Components = components;
        }

        public void Build(IServiceCollection services)
        {
            services.AddHostedService<Chain.From>(x =>
            {
                ILogger logger = x.GetService<ILogger<Route>>();
                logger.LogInformation(Resources.INFO_SETTINGUPROUTE);
                IChain<Routable> next = null;
                if (this.Components != null && this.Components.Count > 0)
                {
                    next = SetupChain(0, logger);
                }
                else
                {
                    next = new Chain.To(logger, this.To);
                }

                return new Chain.From(this.From, next, logger);
            });
        }

        public Builder Error(IErrorComponent errorComponent)
        {
            throw new NotImplementedException("Currently not implemented.");
        }

        private IChain<Routable> SetupChain(int i, ILogger logger)
        {
            Chain.Process chain = new Chain.Process(logger, this.Components[i]);
            logger.LogInformation(string.Format(Resources.INFO_PROCESSINGCOMPONENT, this.Components[i].ToString()));
            if (i < this.Components.Count - 1)
            {
                chain.SetNext(this.SetupChain(++i, logger));
            }
            else
            {
                chain.SetNext(new Chain.To(logger, this.To));
            }

            return chain;
        }
    }
}