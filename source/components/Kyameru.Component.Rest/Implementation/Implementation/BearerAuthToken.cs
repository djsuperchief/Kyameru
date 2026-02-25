using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;

namespace Kyameru.Component.Rest.Implementation
{
    public class BearerAuthToken : IAuthStrategy
    {
        private readonly string _token;

        public BearerAuthToken(string token)
        {
            _token = token;
        }
        
        public Task ApplyAsync(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return Task.CompletedTask;
        }
    }
}