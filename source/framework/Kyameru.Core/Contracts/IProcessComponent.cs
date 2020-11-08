using Kyameru.Core.Contracts;
using System;

namespace Kyameru
{
    public interface IProcessComponent : IComponent
    {
        void Process(Core.Entities.Routable routable);
    }
}