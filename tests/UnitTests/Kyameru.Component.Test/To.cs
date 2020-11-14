using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyameru.Component.Test
{
    public class To : IToComponent
    {
        private readonly Dictionary<string, string> Headers;

        public event EventHandler<Log> OnLog;

        public To(Dictionary<string, string> headers)
        {
            this.Headers = headers;
        }

        public void Process(Routable item)
        {
            if (item.Headers["Target"] == "kyameru")
            {
                item.SetInError(new Error("To", "Process", "Error"));
                this.OnLog?.Invoke(this, new Log(LogLevel.Error, "Error", new ArgumentException("Error")));
            }
            this.OnLog?.Invoke(this, new Log(LogLevel.Information, "To Executed"));
        }
    }
}