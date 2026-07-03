namespace Kyameru.TestUtilities.Contracts;

public interface ILocalstackResource
{
    Task Create(string name, Dictionary<string, string> props);
    
    Task Delete(string name, Dictionary<string, string> props);
}