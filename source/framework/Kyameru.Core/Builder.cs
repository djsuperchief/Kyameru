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
            List<Contracts.IProcessComponent> components,
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
            if(this.Components != null && this.Components.Count > 0)
            {
                next = new Chain.Process(null, this.Components[0]);
                for (int i = 1; i < this.Components.Count; i++)
                {
                    // this needs working on
                    // setup the chain here
                }
                next.SetNext(new Chain.To(null, this.To));
            }
            else
            {
                next = new Chain.To(null, this.To);
            }
            

            // finish the to chain here.

            services.AddHostedService<Chain.From>(x =>
            {
                return new Chain.From(this.From, next);
            });
        }

    }
}
