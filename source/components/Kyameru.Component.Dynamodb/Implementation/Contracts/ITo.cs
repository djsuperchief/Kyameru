using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Dynamodb.Contracts
{
    public interface ITo : IToChainLink
    {
        void SetHeaders(Dictionary<string, string> headers);
    }
}