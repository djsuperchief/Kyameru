﻿using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyameru.Component.Test
{
    public class From : IFromComponent
    {
        public event EventHandler<Routable> OnAction;

        public event EventHandler<Log> OnLog;

        private readonly Dictionary<string, string> headers;

        public From(Dictionary<string, string> headers)
        {
            this.headers = headers;
        }

        public void Setup()
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Setup"));
        }

        public void Start()
        {
            Routable routable = new Routable(this.headers, "TestData");
            GlobalCalls.Calls.Add("FROM");
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "FROM"));
            this.OnAction?.Invoke(this, routable);
        }

        public void Stop()
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Stop"));
        }
    }
}