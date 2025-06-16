using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Sns
{
    public interface ITo : IToChainLink
    {
        void SetHeaders(Dictionary<string, string> headers);
    }
}
