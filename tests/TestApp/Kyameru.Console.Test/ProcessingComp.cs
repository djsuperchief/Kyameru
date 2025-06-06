﻿using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Console.Test
{
    internal class ProcessingComp : IProcessor
    {
        public event EventHandler<Log> OnLog;

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            routable.SetBody<string>("Kyameru testing....async...sorry #notsorry");
            if (this.OnLog != null)
            {
                this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Test"));
            }
            await Task.CompletedTask;
        }
    }
}
