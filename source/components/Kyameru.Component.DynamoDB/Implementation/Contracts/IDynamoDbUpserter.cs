using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.DynamoDB.Entities;

namespace Kyameru.Component.DynamoDB.Contracts
{
    public interface IDynamoDbUpserter
    {
        Task SaveAsync(object entity, string table = "", CancellationToken cancellationToken = default);
        Task SaveAsync(IEnumerable<object>? entities, string table, CancellationToken cancellationToken = default);
    }
}