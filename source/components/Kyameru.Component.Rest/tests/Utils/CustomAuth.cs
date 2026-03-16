using Kyameru.Component.Rest.Contracts;

namespace Kyameru.Component.Rest.Tests.Utils;

public class CustomAuth(string value) :IAuthStrategy
{
    public async Task ApplyAsync(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("CustomAuth", value);
        await Task.CompletedTask;
    }
}