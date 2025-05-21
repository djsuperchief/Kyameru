using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Injectiontest
{
    public interface IMyTo : IToChainLink
    {
        void AddHeaders(Dictionary<string, string> headers);
    }
}
