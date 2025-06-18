using System;
using System.Collections.Generic;

namespace Kyameru.Component.Rest.Contracts
{
    public interface IRestChain
    {
        void SetHeaders(Dictionary<string, string> headers);

        string HttpMethod { get; }

        string Url { get; }
    }
}