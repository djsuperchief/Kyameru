using Kyameru.Component.DynamoDB.Contracts;

namespace Kyameru.Component.DynamoDB.Tests.Entities;

public class TestDbRecord : IDynamoRecord
{
    public object HashKey { get; set; } = Guid.NewGuid();
    public object RangeKey { get; set; } = Guid.NewGuid();
}