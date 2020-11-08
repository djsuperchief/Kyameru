using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Core
{
    public class RouteBuilder
    {
        private Contracts.IFromComponent from;
        private List<Contracts.IProcessComponent> components;
        private string[] fromArgs;
        private string fromString;

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

        public RouteBuilder(string from, string[] args)
        {
            // Components here must be activated by name and therefor must be done
            Type fromType = Type.GetType($"Kyameru.Component.{from}.Inflator, Kyameru.Component.{from}");
            IOasis oasis = (IOasis)Activator.CreateInstance(fromType);
            this.from = oasis.CreateComponent();
            this.fromArgs = args;
        }

        public Builder To(Contracts.IToComponent toComponent)
        {
            return new Builder(from, fromArgs, components, toComponent);
        }

        
    }
}
