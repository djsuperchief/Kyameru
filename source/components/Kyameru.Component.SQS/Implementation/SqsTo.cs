using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Component.SQS
{
    public class SqsTo : IToComponent
    {
        private Dictionary<string, string> headers;

        public event EventHandler<Log> OnLog;

        public SqsTo(Dictionary<string, string> headers)
        {
            this.headers = headers.ParseHeaders();
        }

        public void Process(Routable item)
        {

        }
    }
}

