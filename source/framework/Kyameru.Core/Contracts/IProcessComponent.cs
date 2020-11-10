using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;

namespace Kyameru
{
    public interface IProcessComponent : IComponent
    {
        void Process(Core.Entities.Routable routable);
    }
}