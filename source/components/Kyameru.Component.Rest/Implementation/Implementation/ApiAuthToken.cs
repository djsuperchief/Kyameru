using System.Net.Http;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;

namespace Kyameru.Component.Rest.Implementation
{
    public class ApiAuthToken : IAuthStrategy
    {
        private readonly string _apiHeader;
        private readonly string _apiToken;

        public ApiAuthToken(string token, string header = "X-API-Key")
        {
            _apiToken = token;
            _apiHeader = header;
        }
        
        public Task ApplyAsync(HttpClient client)
        {
            client.DefaultRequestHeaders.Add(_apiHeader, _apiToken);
            return Task.CompletedTask;
        }
    }
}