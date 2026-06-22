using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Dynamodb.Entities;

namespace Kyameru.Component.Dynamodb.Contracts
{
    public interface IDynamoDbUpserter
    {
        Task SaveAsync(object entity, string table = "", CancellationToken cancellationToken = default);
        Task SaveAsync(IEnumerable<object>? entities, string table, CancellationToken cancellationToken = default);
    }
}