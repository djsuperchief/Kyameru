using System;
namespace Kyameru.Core.Contracts
{
    public interface IProcessComponent : IComponent
    {
        void Process(Entities.Routable routable);
    }
}
