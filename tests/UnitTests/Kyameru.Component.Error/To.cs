using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.Error
{
    public class To : BaseError, IToComponent
    {
        public event EventHandler<Log> OnLog;

        public To(Dictionary<string, string> headers) : base(headers)
        {
        }

        public void Process(Routable item)
        {
            if (this.WillError())
            {
                throw new NotImplementedException();
            }
        }
    }
}