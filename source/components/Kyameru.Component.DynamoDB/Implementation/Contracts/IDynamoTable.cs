namespace Kyameru.Component.DynamoDB.Contracts
{
    /// <summary>
    /// Interface for generic DynamoDB table.
    /// </summary>
    public interface IDynamoTable
    {
        /// <summary>
        /// Gets or sets the table hash key
        /// </summary>
        object HashKey { get; set; }
        
        /// <summary>
        /// Gets or sets the table range key
        /// </summary>
        object RangeKey { get; set; }
    }
}