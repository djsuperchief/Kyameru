using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Kyameru.TestUtilities.Contracts;

namespace Kyameru.TestUtilities.Localstack.Resources;

public class DynamoDb(IAmazonDynamoDB dynamoDb) : ILocalstackResource
{
    public async Task Create(string name, Dictionary<string, string> props)
    {
        var keySchema = new List<KeySchemaElement>()
        {
            new()
            {
                AttributeName = props["HashKey"],
                KeyType = "HASH"
            }
        };

        var attributes = new List<AttributeDefinition>()
        {
            new()
            {
                AttributeName = props["HashKey"],
                AttributeType = "S"
            }
        };

        if (props.ContainsKey("RangeKey"))
        {
            keySchema.Add(new KeySchemaElement(props["RangeKey"], KeyType.RANGE));
            attributes.Add(new AttributeDefinition() { AttributeName =  props["RangeKey"], AttributeType = "S" });
        }
        
        var createTableRequest = new CreateTableRequest()
        {
            AttributeDefinitions = attributes,
            KeySchema = keySchema,
            TableName = name,
            ProvisionedThroughput = new ProvisionedThroughput(1, 1),
            StreamSpecification = new StreamSpecification()
            {
                StreamEnabled = true,
                StreamViewType = StreamViewType.NEW_AND_OLD_IMAGES
            }
        };
        
        await dynamoDb.CreateTableAsync(createTableRequest);
    }

    public async Task Delete(string name, Dictionary<string, string> props)
    {
        await dynamoDb.DeleteTableAsync(name, CancellationToken.None);
    }
}