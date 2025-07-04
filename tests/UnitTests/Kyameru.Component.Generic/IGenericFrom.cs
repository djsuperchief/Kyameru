using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Generic;

public interface IGenericFrom : IFromChainLink
{
    Guid Id { get; }

    void SetId(Guid id);
}
