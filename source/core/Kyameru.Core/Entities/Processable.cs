using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core.Entities
{
    public class Processable
    {
        public enum InvocationType
        {
            DI,
            Concrete
        };

        protected Processable(IProcessComponent target)
        {
            this.Invocation = InvocationType.Concrete;
            this.Component = target;
        }

        protected Processable(Type type)
        {
            this.Invocation = InvocationType.DI;
            this.ComponentType = type;
        }

        public InvocationType Invocation { get; private set; }

        public Type ComponentType { get; private set; }

        public IProcessComponent Component { get; set; }

        public static Processable Create<T>() where T : IProcessComponent
        {
            return new Processable(typeof(T));
        }

        public static Processable Create(IProcessComponent component)
        {
            return new Processable(component);
        }

        public IProcessComponent GetComponent(IServiceProvider provider)
        {
            if (this.Invocation == InvocationType.Concrete)
            {
                return this.Component;
            }
            else
            {
                return (IProcessComponent)provider.GetService(this.ComponentType);
            }
        }
    }
}