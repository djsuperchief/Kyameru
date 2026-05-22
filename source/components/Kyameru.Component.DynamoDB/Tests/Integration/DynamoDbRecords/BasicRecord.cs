using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.Entities;
using Newtonsoft.Json;

namespace Kyameru.Component.DynamoDB.IntegrationTests.DynamoDbRecords;

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
}