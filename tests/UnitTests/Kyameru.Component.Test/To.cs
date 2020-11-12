using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
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
            this.OnLog(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "To Executed"));
        }
    }
}