using System;

namespace Kyameru.Component.Dynamodb.Exceptions
{
    public class MissingDynamoTable : Exception
    {
        public MissingDynamoTable(string message) : base(message) { }
    }
}