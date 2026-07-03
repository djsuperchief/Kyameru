using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.Dynamodb.Contracts;
using Kyameru.Component.Dynamodb.Entities;

namespace Kyameru.Component.Dynamodb.Tests.Entities;

public class TestDbRecord : DynamoRecord<string>
{
    [DynamoDBHashKey]
    public override string HashKey { get; set; }
}