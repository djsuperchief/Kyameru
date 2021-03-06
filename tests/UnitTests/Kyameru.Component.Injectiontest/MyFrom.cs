﻿using System;
using System.Collections.Generic;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Injectiontest
{
    public class MyFrom : IMyFrom
    {

        private Dictionary<string, string> headers;

        public event EventHandler<Routable> OnAction;
        public event EventHandler<Log> OnLog;

        public MyFrom()
        {
            
        }

        public void AddHeaders(Dictionary<string, string> headers)
        {
            this.headers = headers;
        }

        public void Setup()
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Setup"));
        }

        public void Start()
        {
            Routable routable = new Routable(this.headers, "InjectedData");
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
