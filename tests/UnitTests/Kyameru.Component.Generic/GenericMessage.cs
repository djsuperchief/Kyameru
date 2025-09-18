using Kyameru.Core.Comms;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Generic;

public class GenericMessage
{
    public string Info { get; private set; }
    public static GenericMessage Create(string data)
    {
        return new GenericMessage() { Info = data };
    }
}