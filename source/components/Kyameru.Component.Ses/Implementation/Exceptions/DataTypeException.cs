using System;

namespace Kyameru.Component.Ses.Exceptions;

public class DataTypeException(string message) : Exception(message)
{
}
