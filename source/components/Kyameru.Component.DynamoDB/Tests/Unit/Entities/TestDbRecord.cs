using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.Entities;

namespace Kyameru.Component.DynamoDB.Tests.Entities;

public class TestDbRecord : DynamoRecord<string>
{
    [DynamoDBHashKey]
    public override string HashKey { get; set; }
}