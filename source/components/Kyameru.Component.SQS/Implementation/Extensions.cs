using System;
using System.Collections.Generic;
using Kyameru.Core.Exceptions;

namespace Kyameru.Component.SQS
{
    public static class Extensions
    {

        public static AwsConfig ParseHeadersToAwsConfig(this Dictionary<string, string> incoming)
        {
            return new AwsConfig(incoming);
        }
    }
}

