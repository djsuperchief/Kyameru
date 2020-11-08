using System;

namespace Kyameru.Core.Contracts
{
    public interface IComponent
    {
        event EventHandler<Entities.Log> OnLog;
    }
}