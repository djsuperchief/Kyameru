using System.Collections.Generic;

namespace Kyameru.Component.Sqs;

public interface ITo : Kyameru.Core.Contracts.IToChainLink
{
    void SetHeaders(Dictionary<string, string> incomingHeaders);
}