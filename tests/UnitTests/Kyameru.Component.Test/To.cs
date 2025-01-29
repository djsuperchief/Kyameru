using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                DoProcessing(routable, "TO");
            }

            await Task.CompletedTask;
        }

        private void DoProcessing(Routable item, string call)
        {
            item.SetHeader("ToExecuted", "true");
            if (this.Headers["Host"] == "kyameru")
            {
                this.OnLog.Invoke(this, new Log(LogLevel.Warning, "Will not process"));
                item.SetInError(new Error("To", "Process", "Error"));
                GlobalCalls.AddCall(item.Headers["TestName"], call);
                this.OnLog?.Invoke(this, new Log(LogLevel.Error, "Error", new ArgumentException("Error")));
            }

            if (item.Headers.ContainsKey("SetExit") && item.Headers["SetExit"] == "true")
            {
                item.SetExitRoute("Exit triggered");
            }

            GlobalCalls.AddCall(item.Headers["TestName"], call);

            this.OnLog?.Invoke(this, new Log(LogLevel.Information, call));
        }
    }
}