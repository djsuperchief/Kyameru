using System;
using System.Collections.Generic;
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

        public Builder(Contracts.IFromComponent from,
            List<IProcessComponent> components,
            Contracts.IToComponent to)
        {
            this.From = from;
            this.To = to;
            this.Components = components;
        }

        public void Build(IServiceCollection services, ILogger logger)
        {
            IChain<Routable> next = null;
            IChain<Routable> final = new Chain.To(logger, this.To);
            if (this.Components != null && this.Components.Count > 0)
            {
                next = SetupChain(0, logger);
            }
            else
            {
                next = final;
            }

            // finish the to chain here.

            services.AddHostedService<Chain.From>(x =>
            {
                return new Chain.From(this.From, next, x.GetService<ILogger>());
            });
        }

        private IChain<Routable> SetupChain(int i, ILogger logger)
        {
            Chain.Process chain = new Chain.Process(logger, this.Components[i]);
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