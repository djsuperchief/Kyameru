using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Generic;

public interface IGenericFrom : IFromChainLink
{
    string Id { get; }

    void SetId(string id);
}
