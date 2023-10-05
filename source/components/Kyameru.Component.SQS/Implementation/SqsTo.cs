using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Component.SQS
{
    public class SqsTo : IToComponent
    {
        //private Dictionary<string, string> headers;

        private AwsConfig awsConfig;

        public event EventHandler<Log> OnLog;

        public SqsTo(Dictionary<string, string> headers)
        {
            awsConfig = headers.ParseHeadersToAwsConfig();
        }

        public void Process(Routable item)
        {

        }
    }
}

