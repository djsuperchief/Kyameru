using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.Entities;
using Newtonsoft.Json;

namespace Kyameru.Component.DynamoDB.IntegrationTests.DynamoDbRecords;

[DynamoDBTable("KyameruTestFrom")]
public class BasicRecord : DynamoRecord<string, string>
{
    [JsonPropertyName("Identity")]
    public override string HashKey { get; set; }
    
    [JsonPropertyName("Title")]
    public override string RangeKey { get; set; }

    public string Contents { get; set; }

    public BasicRecord() { }

    public BasicRecord(string contents)
    {
        Contents = contents;
        HashKey = Guid.NewGuid().ToString();
        RangeKey = Guid.NewGuid().ToString();
        
    }

    
}