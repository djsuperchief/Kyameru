using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.Dynamodb.Entities;

namespace Kyameru.Component.Dynamodb.IntegrationTests.DynamoDbRecords;

[ExcludeFromCodeCoverage]
[DynamoDBTable("KyameruTestFrom")]
public class BasicRecord : DynamoRecord<string>
{
    [JsonPropertyName("Department")]
    public override string HashKey { get; set; }

    public string Identity { get; set; }
    
    public string Contents { get; set; }

    public BasicRecord() { }

    public BasicRecord(string contents, string department)
    {
        Contents = contents;
        HashKey = department;
        Identity = Guid.NewGuid().ToString();
    }

    public override bool Equals(object? obj)
    {
        if (obj is BasicRecord other)
        {
            return HashKey == other.HashKey
                && Identity == other.Identity
                && Contents ==  other.Contents;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HashKey, Identity, Contents);
    }
}