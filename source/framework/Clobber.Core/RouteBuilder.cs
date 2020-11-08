using System;
using System.Collections.Generic;

namespace Kyameru.Core
{
    public class RouteBuilder
    {
        private Contracts.IFromComponent from;
        private List<Contracts.IProcessComponent> components;
        private string[] fromArgs;

        public RouteBuilder(Contracts.IFromComponent fromComponent)
        {
            this.from = fromComponent;
        }

        public RouteBuilder(Contracts.IFromComponent fromComponent, string[] args) : this(fromComponent)
        {
            this.fromArgs = args;
        }

        public RouteBuilder Process(Contracts.IProcessComponent processComponent)
        {
            this.components.Add(processComponent);

            return this;
        }

        public Builder To(Contracts.IToComponent toComponent)
        {
            return new Builder(from, components, toComponent);
        }

        
    }
}
