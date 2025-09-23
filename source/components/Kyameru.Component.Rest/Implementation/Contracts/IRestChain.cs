using System.Collections.Generic;
using System.Net.Http;

namespace Kyameru.Component.Rest.Contracts
{
    public interface IRestChain
    {
        void SetHeaders(Dictionary<string, string> headers);
        
        HttpMethod HttpMethod { get; }
        
        string Url { get; }
    }
}