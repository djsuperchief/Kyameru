using System.Net.Http;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Rest.Contracts
{
    public interface IHttpContentFactory
    {
        HttpContent Create(Routable routable);
    }
}