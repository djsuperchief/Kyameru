using System;

namespace Kyameru.Component.SQS.Exceptions
{
    public class MissingRequiredHeaderException : Exception
    {
        public MissingRequiredHeaderException(string message) : base(message) { }
    }
}
