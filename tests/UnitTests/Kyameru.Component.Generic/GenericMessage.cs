using Kyameru.Core.Contracts;

namespace Kyameru.Component.Generic;

public class GenericMessage : IRouteCommsMessage
{
    public Guid MessageId { get; private init; }
    public object Data { get; private init; }

    public static GenericMessage Create(string data)
    {
        return new GenericMessage() { MessageId = Guid.NewGuid(), Data = data };
    }
}