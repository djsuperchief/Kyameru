using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kyameru.Testable.Service.DummyComponents
{
    public class ErrorHandler : IErrorComponent
    {
        public event EventHandler<Log> OnLog;

        public void Process(Routable item)
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Error, "I have been called, i am your error component"));
        }
    }
}