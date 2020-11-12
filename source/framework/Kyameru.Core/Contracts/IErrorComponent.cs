using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    public interface IErrorComponent : IComponent
    {
        void Process(Routable item);
    }
}