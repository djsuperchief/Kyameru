using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

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

        public void Build()
        {
        }

        public void Build(IServiceCollection services)
        {
            IChain<Routable> next = null;
            IChain<Routable> final = new Chain.To(null, this.To);
            if (this.Components != null && this.Components.Count > 0)
            {
                next = SetupChain(0);
            }
            else
            {
                next = final;
            }

            // finish the to chain here.

            services.AddHostedService<Chain.From>(x =>
            {
                return new Chain.From(this.From, next);
            });
        }

        private IChain<Routable> SetupChain(int i)
        {
            Chain.Process chain = new Chain.Process(null, this.Components[i]);
            if (i < this.Components.Count - 1)
            {
                chain.SetNext(this.SetupChain(++i));
            }
            else
            {
                chain.SetNext(new Chain.To(null, this.To));
            }

            return chain;
        }
    }
}