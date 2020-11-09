using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core.BaseComponents
{
    public class AddHeader : IProcessComponent
    {
        public event EventHandler<Log> OnLog;

        private readonly string header;
        private readonly string value;
        private readonly Func<string> callback = null;

        public AddHeader(string header, string value)
        {
            this.header = header;
            this.value = value;
        }

        public AddHeader(string header, Func<string> callbackOne)
        {
            this.header = header;
        }

        public void Process(Routable routable)
        {
            if (!string.IsNullOrWhiteSpace(this.value))
            {
                routable.AddHeader(this.header, this.value);
            }
            else
            {
                routable.AddHeader(this.header, this.callback());
            }
        }
    }
}