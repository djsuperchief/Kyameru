using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Tests.ExchangeAndRouter;

public class TestMessage 
{
    public string Name { get; set; }

    public TestMessage(string name)
    {
        this.Name = name;
    }
}