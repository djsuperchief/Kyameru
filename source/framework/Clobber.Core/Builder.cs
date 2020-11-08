using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;

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

    }
}
