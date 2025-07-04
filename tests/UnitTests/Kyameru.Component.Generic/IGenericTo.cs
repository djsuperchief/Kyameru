using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Generic;

public interface IGenericTo : IToChainLink
{
    Guid Id { get; }
}
