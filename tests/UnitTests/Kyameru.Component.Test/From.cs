using Kyameru.Core.Contracts;
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
            // do nothing
        }

        public void Start()
        {
            Routable routable = new Routable(this.headers, "TestData");
            this.OnAction?.Invoke(this, routable);
        }

        public void Stop()
        {
            // do nothing
        }
    }
}