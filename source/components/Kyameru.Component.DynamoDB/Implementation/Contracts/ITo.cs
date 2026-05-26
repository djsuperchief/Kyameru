using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.DynamoDB.Contracts
{
    public interface ITo : IToChainLink
    {
        void SetHeaders(Dictionary<string, string> headers);
    }
}