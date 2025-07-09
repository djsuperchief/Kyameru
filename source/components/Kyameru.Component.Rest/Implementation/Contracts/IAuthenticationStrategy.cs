using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kyameru.Component.Rest.Contracts
{
    public interface IAuthenticationStrategy
    {
        Task ApplyAsyc(HttpClient httpClient);
    }
}