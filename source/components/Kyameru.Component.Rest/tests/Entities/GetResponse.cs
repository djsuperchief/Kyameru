namespace Kyameru.Component.Rest.Tests.Entities;

public class GetResponse
{
    public string Method { get; set; }
    public string Url { get; set; }

    public object Data { get; set; }

    public async Task<string> GetStringContentBody()
    {
        return await ((StringContent)Data).ReadAsStringAsync();
    }
}