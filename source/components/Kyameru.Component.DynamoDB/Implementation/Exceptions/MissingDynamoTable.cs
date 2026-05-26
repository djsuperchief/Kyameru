using System;

namespace Kyameru.Component.DynamoDB.Exceptions
{
    public class MissingDynamoTable : Exception
    {
        public MissingDynamoTable(string message) : base(message) { }
    }
}