using System;

namespace Kyameru.Component.SQS.Exceptions;

public class MissingHeaderException(string message) : Exception(message);