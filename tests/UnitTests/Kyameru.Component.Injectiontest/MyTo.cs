using System;
using System.Collections.Generic;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Injectiontest
{
    public class MyTo : IMyTo
    {

        private  Dictionary<string, string> headers;

        public MyTo()
        {
            
        }

        public void AddHeaders(Dictionary<string, string> headers)
        {
            this.headers = headers;
        }

        public event EventHandler<Log> OnLog;

        public void Process(Routable item)
        {
            GlobalCalls.Calls.Add("TO");
            item.SetBody<string>("Injected Test Complete");
            this.OnLog?.Invoke(this, new Log(LogLevel.Information, "TO"));
        }
    }
}
