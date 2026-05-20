namespace Kyameru.Component.DynamoDB.Contracts
{
    /// <summary>
    /// Interface for generic DynamoDB table.
    /// </summary>
    public interface IDynamoRecord
    {
        /// <summary>
        /// Gets or sets the table hash key
        /// </summary>
        object HashKey { get; set; }
    }
}