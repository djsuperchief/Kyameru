using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core.Contracts
{
    public interface IErrorComponent : IComponent
    {
        void Process(Routable item);
    }
}