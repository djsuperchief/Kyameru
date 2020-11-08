using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Core
{
    public class Builder
    {
        private readonly IFromComponent From;
        private readonly IToComponent To;
        private readonly string[] fromArgs;
        private readonly List<IProcessComponent> Components;
        

        public Builder(Contracts.IFromComponent from,
            string[] fromArgs,
            List<Contracts.IProcessComponent> components,
            Contracts.IToComponent to)
        {
            this.From = from;
            this.fromArgs = fromArgs;
            this.To = to;
            this.Components = components;
        }

        public void Build()
        {

        }

        public void Build(IServiceCollection services)
        {
            services.AddHostedService<Chain.From>(x =>
            {
                return new Chain.From(this.From, this.fromArgs, null);
            });
        }

    }
}
