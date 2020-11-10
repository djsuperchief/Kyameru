using System;
using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    public interface IFromComponent : IComponent
    {
        event EventHandler<Routable> OnAction;

        void Setup();

        void Start();

        void Stop();
    }
}