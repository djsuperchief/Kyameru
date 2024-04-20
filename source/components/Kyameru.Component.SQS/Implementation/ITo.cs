using System.Collections.Generic;

namespace Kyameru.Component.SQS;

public interface ITo : Kyameru.Core.Contracts.IToComponent
{
    void SetHeaders(Dictionary<string, string> incomingHeaders);
}