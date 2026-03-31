using System.Net.Http;
using System.Threading.Tasks;

namespace Kyameru.Component.Rest.Contracts
{
    public interface IAuthStrategy
    {
        Task ApplyAsync(HttpClient client);
    }
}