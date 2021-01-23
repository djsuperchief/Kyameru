﻿using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.Error
{
    public class Atomic : BaseError, IAtomicComponent
    {
        public Atomic(Dictionary<string, string> headers) : base(headers)
        {
        }

        public event EventHandler<Log> OnLog;

        public void Process(Routable item)
        {
            if (this.WillError())
            {
                this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Error, "Manual error", new NotImplementedException("Manual")));
                throw new Kyameru.Core.Exceptions.ComponentException("Manual Error", new NotImplementedException("Manual"));
            }
        }
    }
}