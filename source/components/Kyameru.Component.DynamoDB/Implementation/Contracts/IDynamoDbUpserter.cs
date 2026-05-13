using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.DynamoDB.Entities;

namespace Kyameru.Component.DynamoDB.Contracts
{
    public interface IDynamoDbUpserter
    {
        Task SaveAsync(object entity, string tableOverride = "", CancellationToken cancellationToken = default);
        Task SaveAsync(IEnumerable<IDynamoRecord>? entities, string tableOverride = "", int batchSize = 50, CancellationToken cancellationToken = default);
    }
}