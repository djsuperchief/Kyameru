using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Core
{
    public class RouteBuilder
    {
        private Contracts.IFromComponent from;
        private List<Contracts.IProcessComponent> components;

        

        



        public RouteBuilder(string from, string[] args)
        {
            // Components here must be activated by name and therefor must be done
            Type fromType = Type.GetType($"Kyameru.Component.{from}.Inflator, Kyameru.Component.{from}");
            IOasis oasis = (IOasis)Activator.CreateInstance(fromType);
            this.from = oasis.CreateFromComponent(args);
            
        }

        public RouteBuilder Process(Contracts.IProcessComponent processComponent)
        {
            if(this.components == null)
            {
                this.components = new List<IProcessComponent>();
            }

            this.components.Add(processComponent);

            return this;
        }

        public Builder To(string to, params string[] args)
        {
            Type toType = Type.GetType($"Kyameru.Component.{to}.Inflator, Kyameru.Component.{to}");
            IOasis oasis = (IOasis)Activator.CreateInstance(toType);
            IToComponent toComponent = oasis.CreateToComponent(args);
            return new Builder(this.from, this.components, toComponent);
        }

        
    }
}
