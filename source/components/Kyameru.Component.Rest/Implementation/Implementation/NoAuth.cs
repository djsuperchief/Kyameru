using System.Net.Http;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;

namespace Kyameru.Component.Rest.Implementation
{
    public class NoAuth : IAuthStrategy
    {
        public async Task ApplyAsync(HttpClient client)
        {
            await Task.CompletedTask;
        }
    }
}