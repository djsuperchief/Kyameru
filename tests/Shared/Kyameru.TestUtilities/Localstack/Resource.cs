namespace Kyameru.TestUtilities.Localstack;

public class Resource
{
    public string Name { get; init; }

    public Enums.LocalstackService ServiceType { get; init; }

    public Dictionary<string,string> Props { get; init; }
}