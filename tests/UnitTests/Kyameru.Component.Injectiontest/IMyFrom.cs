using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Injectiontest
{
    public interface IMyFrom : IFromChainLink
    {
        void AddHeaders(Dictionary<string, string> headers);
    }
}
