using System;

namespace Kyameru.Component.Dynamodb.Exceptions
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string message) : base(message) { }
    }
}