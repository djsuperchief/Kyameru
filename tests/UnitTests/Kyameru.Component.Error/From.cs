using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.Error
{
    public class From : BaseError, IFromComponent
    {
        public event EventHandler<Routable> OnAction;

        public event EventHandler<Log> OnLog;

        public From(Dictionary<string, string> headers) : base(headers)
        {
        }

        public void Setup()
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Setup"));
        }

        public void Start()
        {
            if (this.WillError())
            {
                throw new NotImplementedException();
            }

            Routable routable = new Routable(this.headers, "Test Data");
            this.OnAction?.Invoke(this, routable);
        }

        public void Stop()
        {
            // nothing to do.
        }
    }
}