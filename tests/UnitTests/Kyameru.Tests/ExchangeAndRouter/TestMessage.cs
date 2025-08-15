using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Tests.ExchangeAndRouter;

public class TestMessage : IRouteCommsMessage
{
    public Guid MessageId { get; }
    public object Data { get; }

    public TestMessage(string data)
    {
        this.MessageId = Guid.NewGuid();
        this.Data = data;
    }
}