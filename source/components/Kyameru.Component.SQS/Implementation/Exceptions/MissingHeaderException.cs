using System;

namespace Kyameru.Component.Sqs.Exceptions;

public class MissingHeaderException(string message) : Exception(message);