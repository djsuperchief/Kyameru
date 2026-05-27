using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.DynamoDB.Contracts
{
    public interface IFrom : IFromChainLink
    {
        void SetHeaders(Dictionary<string, string> headers);
    }
}