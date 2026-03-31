using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;

namespace Kyameru.Component.Rest.Implementation
{
    public class BasicAuthToken : IAuthStrategy
    {
        private readonly string _username;
        private readonly string _password;

        public BasicAuthToken(string username, string password)
        {
            _username = username;
            _password = password;
        }
        
        public Task ApplyAsync(HttpClient client)
        {
            var credentialBytes = Encoding.ASCII.GetBytes($"{_username}:{_password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentialBytes));
            return Task.CompletedTask;
        }
    }
}