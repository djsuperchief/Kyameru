using System;

namespace Kyameru.Component.Ses.Exceptions;

public class MissingInformationException(string message) : Exception(message)
{
}
