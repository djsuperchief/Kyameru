using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Injectiontest
{
    public interface IMyTo : IToComponent
    {
        void AddHeaders(Dictionary<string, string> headers);
    }
}
