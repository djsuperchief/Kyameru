using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Ses;

public interface ITo : IToChainLink
{
    void SetHeaders(Dictionary<string, string> incomingHeaders);
}
