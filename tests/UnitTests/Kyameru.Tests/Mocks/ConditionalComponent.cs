using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Tests;

public class ConditionalComponent : IConditionalComponent
{
    public bool Execute(Routable routable)
    {
        throw new NotImplementedException();
    }
}
