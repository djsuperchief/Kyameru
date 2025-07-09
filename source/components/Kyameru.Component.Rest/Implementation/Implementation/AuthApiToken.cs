using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;

namespace Kyameru.Component.Rest.Implementation
{
    public class AuthApiToken : IAuthenticationStrategy
    {
        private readonly string _apiToken;
        private readonly string _apiHeader;

        public AuthApiToken(string token, string header = "X-API-Key")
        {
            _apiToken = token;
            _apiHeader = header;
        }
        public Task ApplyAsyc(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add(_apiHeader, _apiToken);
            return Task.CompletedTask;
        }
    }
}