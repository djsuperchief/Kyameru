﻿using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Kyameru.Tests.Mocks
{
    public class MyPostComponent : IMyComponent
    {
        public event EventHandler<Log> OnLog;

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                this.OnLog?.Invoke(this, new Log(LogLevel.Information, "MyPostComponent has processed Async"));
                this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Setting header"));
                routable.SetHeader("ComponentRan", "Yes");
            }



            await Task.CompletedTask;
        }
    }
}