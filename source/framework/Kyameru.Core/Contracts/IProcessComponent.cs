using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru
{
    public interface IProcessComponent : IComponent
    {
        void Process(Routable routable);
    }
}